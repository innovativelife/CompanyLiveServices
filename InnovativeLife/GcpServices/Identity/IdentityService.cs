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
using InnovativeLife.Services.Employee;

namespace InnovativeLife.GcpServices.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    // private readonly IEmployeeService _employeeService;
    public IdentityService(ILogger<IdentityService> logger)//, IEmployeeService employeeService)
    {
        _logger = logger;
        // _employeeService = employeeService;
    }

    // Use google's Auth API to validate the bearer token, and that the user is valid for tenant
    public async Task<AuthenticateResult> AuthenticateUserAndTenant(string authToken, string tenantId, IUserContext userContext, string schemeName)
    {
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to validate token for user {userContext.uId} and tenant {tenantId}", LogLevel.Information);

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
            _logger.LogInformation("IdentityService.AuthenticateUserAndTenant: Skipping token validation - user is in dev mode");

            userContext.SetDevelopmentModeContext();
        }
        else
        {
            var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);
            var validateResult = await ValidateToken(authToken, authManager);

            if (!validateResult.Item1)
            {
                return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Token validation failed");
            }

            var decodedToken = validateResult.Item2;

            _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Verify passed ok");

            if (!await GetUserAndTenant(tenantId, authManager, decodedToken, userContext))
            {
                return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Failed to retrieve user or tenant");
            }

            // // Get Employee Details
            // var getEmployeeResult = await _employeeService.ReadByEmployeeUID(userContext, userContext.uId);
            // if (!getEmployeeResult.Success)
            // {
            //     _logger.LogError($"Failed to retrieve employee details for uid: {userContext.uId}");
            //     return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: failed to retribe employee details for user");
            // }

            // userContext.adminPrivilege = getEmployeeResult.employee!.adminPrivilege;
        }

        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to get claims from userContext for uId: {userContext.uId}");
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
            _logger.LogInformation($"IdentityService.ValidateToken: About to call VerifyIdTokenAsync for token with length {authToken.Length}");
            decodedToken = await authManager.VerifyIdTokenAsync(authToken);

            if (decodedToken == null)
            {
                _logger.LogInformation("IdentityService.ValidateToken: Token validation fialed - decoded token is null");
                return new Tuple<bool, string>(false, "");
            }

            return new Tuple<bool, string>(true, decodedToken.Uid); ;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"IdentityService.ValidateToken: Authentication failed with exception during token validation: {ex.Message}");
            return new Tuple<bool, string>(false, "");
        }
    }

    // Get User and Tenant Details
    private async Task<bool> GetUserAndTenant(string tenantId, TenantAwareFirebaseAuth authManager, string uId, IUserContext userContext)
    {
        try
        {
            _logger.LogInformation($"IdentityService.GetUserAndTenant: Executing GetUserAndTenant for tenantID: {tenantId} Uid: {uId}");
            UserRecord userRecord = await authManager.GetUserAsync(uId);

            if (userRecord == null)
            {
                _logger.LogInformation($"IdentityService.GetUserAndTenant: Could not get user record from GCP");
                return false;
            }

            _logger.LogInformation($"IdentityService.GetUserAndTenant: Retrieved user details - UiD:         {userRecord.Uid}");
            _logger.LogInformation($"IdentityService.GetUserAndTenant: Retrieved user details - TenantId:    {userRecord.TenantId}");

            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.GetTenantAsync(tenantId);

            _logger.LogInformation($"IdentityService.GetUserAndTenant: Retrieved tenant details - DisplayName: {tenant.DisplayName}");

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
            _logger.LogError($"IdentityService.GetUserAndTenant: Error attempting to read user or tenant record: {ex.Message}");
            return false;
        }
    }

    public async Task<AddTenantResponse> AddTenant(string displayName)
    {
        try
        {
            _logger.LogInformation($"IdentityService.AddTenant: Attempting to add tenant with displayName {displayName}");
            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.CreateTenantAsync(new TenantArgs()
            {
                DisplayName = displayName,
                EmailLinkSignInEnabled = true,
                PasswordSignUpAllowed = true
            });

            var response = new AddTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant Successfully Added to Identity Manager");
            response.tenantId = tenant.TenantId;

            _logger.LogInformation($"IdentityService.AddTenant: Finished adding tenant with displayName {displayName}");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"IdentityService.AddTenant: Error while trying to create Tenant in Identity Manager = {ex.Message}");
            return new AddTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    public async Task<AddUserToTenantResponse> AddUserToTenant(string tenantId, string displayName, string email, string phoneNumber, string initialPassword, IUserContext requestContext)
    {
        try
        {
            _logger.LogInformation($"IdentityService.AddUserToTenant: Attempting to add user {displayName} to tenant {tenantId}");

            if (requestContext.rootAdmin)
            {
                _logger.LogWarning($"IdentityService.AddUserToTenant: Service being run by RootAdmin user {requestContext.uId} in tenant {requestContext.tenantId}");
            }

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

            _logger.LogInformation($"IdentityService.AddUserToTenant: About to create user in tenant");
            var result = await authManager.CreateUserAsync(userRecord);

            if (result == null)
            {
                _logger.LogError($"IdentityService.AddUserToTenant: null result returned from CreateUserAsync");
                return new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Exception, "Error creating user - null returned");
            }

            _logger.LogInformation($"IdentityService.AddUserToTenant: User created with Uid: {result.Uid}");

            var response = new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "");
            response.uId = result.Uid;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"IdentityService.AddUserToTenant: Error creating user for tenant");
            return new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    public async Task<Tuple<bool, string>> SetAdminAuthorisationForUser(string tenantId, string uid, bool adminUser, IUserContext requestContext)
    {
        try
        {
            _logger.LogInformation($"IdentityService.SetAdminAuthorisationForUser: Starting for {uid} to {adminUser}");

            var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);

            // Set admin privileges on the user corresponding to uid.
            var claims = new Dictionary<string, object>()
            {
                { "admin", adminUser },
            };

            _logger.LogInformation($"IdentityService.SetAdminAuthorisationForUser: Setting admin custom claim for {uid} to {adminUser}");
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

            return new Tuple<bool, string>(true, "");
        }
        catch (Exception ex)
        {
            _logger.LogError($"IdentityService.SetAdminAuthorisationForUser: Error attempting to add customer claims");
            return new Tuple<bool, string>(false, GetErrorMessageFromGcpAuthError(ex));
        }
    }

    // GCP has very unhelpful error handling for Auth errors, throwing an exception with a message that contains a code inside an embedded JSON object
    // See here: https://firebase.google.com/docs/auth/admin/errors
    private string GetErrorMessageFromGcpAuthError(Exception exception)
    {
        _logger.LogInformation($"IdentityServiceGetErrorMessageFromGcpAuthError: Translating Firebase Auth Exception: {exception.Message}");

        var gcpMessage = exception.Message.ToLower();
        var message = "Unknown Error from Firebase Auth occurred";

        if (gcpMessage.Contains("email_exists"))
        {
            message = "The provided email is already in use by an existing user. Each user must have a unique email.";
        }

        if (gcpMessage.Contains("id_token_expired"))
        {
            message = "The Firebase ID token has been revoked.";
        }

        if (gcpMessage.Contains("invalid_email"))
        {
            message = "The provided value for the email user property is invalid. It must be a string email address.";
        }

        if (gcpMessage.Contains("invalid_id_token"))
        {
            message = "The provided ID token is not a valid Firebase ID token.";
        }

        if (gcpMessage.Contains("invalid_password"))
        {
            message = "The provided value for the password user property is invalid. It must be a string with at least six characters.";
        }

        if (gcpMessage.Contains("invalid_password"))
        {
            message = "The provided value for the password user property is invalid. It must be a string with at least six characters.";
        }

        if (gcpMessage.Contains("invalid_phone_number"))
        {
            message = "The provided value for the phoneNumber is invalid. It must be a non-empty E.164 standard compliant identifier string.";
        }

        if (gcpMessage.Contains("invalid_photo_url"))
        {
            message = "The provided value for the photoURL user property is invalid. It must be a string URL.";
        }

        if (gcpMessage.Contains("invalid_uid"))
        {
            message = "The provided uid must be a non-empty string with at most 128 characters.";
        }

        if (gcpMessage.Contains("phone_number_exists"))
        {
            message = "The provided phoneNumber is already in use by an existing user. Each user must have a unique phoneNumber.";
        }

        if (gcpMessage.Contains("uid_already_exists"))
        {
            message = "The provided uid is already in use by an existing user. Each user must have a unique uid.";
        }

        if (gcpMessage.Contains("user_not_found"))
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