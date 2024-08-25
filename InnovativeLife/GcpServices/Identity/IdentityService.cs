using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using InnovativeLife.Common;
using InnovativeLife.Security;
using Microsoft.Extensions.Logging;
using FirebaseAdmin.Auth.Multitenancy;
using InnovativeLife.GcpServices.Identity.ServiceMessages;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InnovativeLife.GcpServices.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    public IdentityService(ILogger<IdentityService> logger)
    {
        _logger = logger;
    }

    // Use google's Auth API to validate the bearer token, and that the user is valid for tenant
    public async Task<AuthenticateResult> AuthenticateUserAndTenant(string authToken, string tenantId, IUserContext userContext, string schemeName)
    {
        _logger.LogInformation($"About to validate token for user {userContext.uId} and tenant {tenantId}", LogLevel.Information);

        if (FirebaseAuth.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = GcpConstants.ProjectId,
            });
        }

        if (userContext.developmentMode)
        {
            _logger.LogInformation("Skipping token validation - user is in dev mode");

            userContext.SetDevelopmentModeContext();
        }
        else
        {
            var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);
            var validateResult = await ValidateToken(authToken, authManager);

            if (!validateResult.Item1)
            {
                return AuthenticateResult.Fail("Token validation failed");
            }

            var decodedToken = validateResult.Item2;

            _logger.LogInformation($"Verify passed ok");

            if (! await GetUserAndTenant(tenantId, authManager, decodedToken, userContext))
            {
                 return AuthenticateResult.Fail("Failed to retrieve user or tenant");
            }
        }

        _logger.LogInformation($"About to get claims from userContext for uId: {userContext.uId}");
        var claims = AuthorizationPolicies.GetClaims(userContext, _logger);
        var claimsIdentity = new ClaimsIdentity(claims, schemeName);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, schemeName));
    }

    // Validate the bearer token
    private async Task<Tuple<bool, string>> ValidateToken(string authToken, TenantAwareFirebaseAuth authManager)
    {
        FirebaseToken decodedToken;
        try
        {
            _logger.LogInformation($"About to call VerifyIdTokenAsync for token with length {authToken.Length}");
            decodedToken = await authManager.VerifyIdTokenAsync(authToken);

            if (decodedToken == null)
            {
                _logger.LogInformation("Token validation fialed - decoded token is null");
                return new Tuple<bool, string>(false, "");
            }

            return new Tuple<bool, string>(true, decodedToken.Uid); ;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Authentication failed with exception during token validation: {ex.Message}");
            return new Tuple<bool, string>(false, "");
        }
    }

    // Get User and Tenant Details
    private async Task<bool> GetUserAndTenant(string tenantId, TenantAwareFirebaseAuth authManager, string uId, IUserContext userContext)
    {
        try
        {
            _logger.LogInformation($"Executing GetUserAndTenant for tenantID: {tenantId} Uid: {uId}");
            UserRecord userRecord = await authManager.GetUserAsync(uId);

            if (userRecord == null)
            {
                _logger.LogInformation($"Could not get user record");
                return false;
            }

            _logger.LogInformation($"Retrieved user details - UiD:         {userRecord.Uid}");
            _logger.LogInformation($"Retrieved user details - DisplayName: {userRecord.DisplayName}");
            _logger.LogInformation($"Retrieved user details - TenantId:    {userRecord.TenantId}");
            _logger.LogInformation($"Retrieved user details - Disabled:    {userRecord.Disabled}");
            _logger.LogInformation($"Retrieved user details - Email:       {userRecord.Email}");
            _logger.LogInformation($"Retrieved user details - PhoneNumber: {userRecord.PhoneNumber}");

            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.GetTenantAsync(tenantId);

            _logger.LogInformation($"Retrieved tenant details - DisplayName: {tenant.DisplayName}");

            userContext.uId = userRecord.Uid;
            userContext.displayName = userRecord.DisplayName;
            userContext.active = !userRecord.Disabled;
            userContext.email = userRecord.Email;
            userContext.phoneNumber = userRecord.PhoneNumber;
            userContext.tenantId = userRecord.TenantId;
            userContext.tenantName = tenant.DisplayName;
            userContext.rootAdmin = tenant.TenantId == GcpConstants.RootTenantId;

            _logger.LogInformation($"Root Tenant? {userContext.rootAdmin}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error attempting to read user or tenant record: {ex.Message}");
            return false;
        }
    }

    public async Task<AddTenantResponse> AddTenant(string displayName)
    {
        try
        {
            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.CreateTenantAsync(new TenantArgs()
            {
                DisplayName = displayName,
                EmailLinkSignInEnabled = true,
                PasswordSignUpAllowed = true
            });

            var response = new AddTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant Successfully Added to Identity Manager");
            response.tenantId = tenant.TenantId;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error while trying to create Tenant in Identity Manager");
            return new AddTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    public async Task<AddUserToTenantResponse> AddUserToTenant(string tenantId, string displayName, string email, string phoneNumber, string initialPassword)
    {
        try
        {
            var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);

            var userRecord = new UserRecordArgs()
            {
                DisplayName = displayName,
                Email = email,
                EmailVerified = false,
                PhoneNumber = phoneNumber,
                Password = initialPassword,
                PhotoUrl = "https://Myphoto.com/image.jpeg",
                Disabled = false,
            };

            _logger.LogInformation($"About to create user in tenant");
            var result = await authManager.CreateUserAsync(userRecord);

            if (result == null)
            {
                _logger.LogError($"null result returned from CreateUserAsync");
                return new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Exception, "Error creating user - null returned");
            }

            _logger.LogInformation($"User created with Uid: {result.Uid}");

            var response = new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "");
            response.uId = result.Uid;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Error creating user for tenant");
            return new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    public async Task<Tuple<bool, string>> SetAdminAuthorisationForUser(string tenantId, string uid, bool adminUser)
    {
        try
        {
            var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);

            // Set admin privileges on the user corresponding to uid.
            var claims = new Dictionary<string, object>()
            {
                { "admin", adminUser },
            };

            _logger.LogInformation($"Setting admin custom claim for {uid} to {adminUser}");
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

            return new Tuple<bool, string>(true, "");
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Error attempting to add customer claims");
            return new Tuple<bool, string>(false, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    // GCP has very unhelpful error handling for Auth errors, throwing an exception with a message that contains a code inside an embedded JSON object
    // See here: https://firebase.google.com/docs/auth/admin/errors
    private string GetErrorMessageFromGcpAuthError(Exception exception)
    {
        _logger.LogInformation($"Translating Firebase Auth Exception: {exception.Message}");

        var gcpMessage = exception.Message.ToLower();
        var message = "Unknown Error from Firebase Auth occurred";

        if (gcpMessage.Contains("email-already-exists"))
        {
            message = "The provided email is already in use by an existing user. Each user must have a unique email.";
        }

        if (gcpMessage.Contains("id-token-expired"))
        {
            message = "The Firebase ID token has been revoked.";
        }

        if (gcpMessage.Contains("invalid-email"))
        {
            message = "The provided value for the email user property is invalid. It must be a string email address.";
        }

        if (gcpMessage.Contains("invalid-id-token"))
        {
            message = "The provided ID token is not a valid Firebase ID token.";
        }

        if (gcpMessage.Contains("invalid-password"))
        {
            message = "The provided value for the password user property is invalid. It must be a string with at least six characters.";
        }

        if (gcpMessage.Contains("invalid-password"))
        {
            message = "The provided value for the password user property is invalid. It must be a string with at least six characters.";
        }

        if (gcpMessage.Contains("invalid-phone-number"))
        {
            message = "The provided value for the phoneNumber is invalid. It must be a non-empty E.164 standard compliant identifier string.";
        }

        if (gcpMessage.Contains("invalid-photo-url"))
        {
            message = "The provided value for the photoURL user property is invalid. It must be a string URL.";
        }

        if (gcpMessage.Contains("invalid-uid"))
        {
            message = "The provided uid must be a non-empty string with at most 128 characters.";
        }

        if (gcpMessage.Contains("phone-number-already-exists"))
        {
            message = "The provided phoneNumber is already in use by an existing user. Each user must have a unique phoneNumber.";
        }

        if (gcpMessage.Contains("uid-already-exists"))
        {
            message = "The provided uid is already in use by an existing user. Each user must have a unique uid.";
        }

        if (gcpMessage.Contains("user-not-found"))
        {
            message = "There is no existing user record corresponding to the provided identifier.";
        }

        if (gcpMessage.Contains("invalid_display_name"))
        {
            message = "Tenant display name should start with a letter and consistent of letters, digits and hyphens and be 4 to 20 characters in length.";
        }

        return message;
    }
}