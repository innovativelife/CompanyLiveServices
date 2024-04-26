using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using InnovativeLife.Common;
using Microsoft.Extensions.Logging;
using Firebase.Auth;
using FirebaseAdmin.Auth.Multitenancy;
using InnovativeLife.GcpServices.Identity.ServiceMessages;

namespace InnovativeLife.GcpServices.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    public IdentityService(ILogger<IdentityService> logger)
    {
        _logger = logger;
    }

    // Use google's Auth API to validate the bearer token, and that the user is valid for tenant
    public async Task<Tuple<bool, RequestContext?>> AuthenticateUserAndTenant(string authToken, string tenantId)
    {
        _logger.LogInformation("About to validate token for user and tenant");

        if (FirebaseAuth.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = GcpConstants.ProjectId,
            });
        }

        var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(tenantId);
        var validateResult = await ValidateToken(authToken, tenantId, authManager);


        if (!validateResult.Item1)
        {
            return new Tuple<bool, RequestContext?>(false, null);
        }

        var decodedToken = validateResult.Item2;
        if (decodedToken == null)
        {
            _logger.LogInformation($"Could not get user or tenant");
            return new Tuple<bool, RequestContext?>(false, null);
        }

        _logger.LogInformation($"Verify passed ok");

        return await GetUserAndTenant(tenantId, authManager, decodedToken);
    }

    // Validate the bearer token
    private async Task<Tuple<bool, FirebaseToken?>> ValidateToken(string authToken, string tenantId, FirebaseAdmin.Auth.Multitenancy.TenantAwareFirebaseAuth authManager)
    {
        FirebaseToken decodedToken;
        try
        {
            _logger.LogInformation($"About to call VerifyIdTokenAsync for token with length {authToken.Length}");
            decodedToken = await authManager.VerifyIdTokenAsync(authToken);
            return new Tuple<bool, FirebaseToken?>(true, decodedToken);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Authentication failed: {ex.Message}");
            return new Tuple<bool, FirebaseToken?>(false, null);
        }
    }

    // Get User and Tenant Details
    private async Task<Tuple<bool, RequestContext?>> GetUserAndTenant(string tenantId, TenantAwareFirebaseAuth authManager, FirebaseToken decodedToken)
    {
        try
        {
            _logger.LogInformation($"Uid: {decodedToken.Uid}");
            UserRecord userRecord = await authManager.GetUserAsync(decodedToken.Uid);

            if (userRecord == null)
            {
                _logger.LogInformation($"Could not get user record");
                return new Tuple<bool, RequestContext?>(false, null);
            }

            _logger.LogInformation($"Retrieved user details - Email: {userRecord.Email}");

            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.GetTenantAsync(tenantId);

            return new Tuple<bool, RequestContext?>(true, InitialiseRequestContext(userRecord, tenant));
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Error attempting to read user or tenant record: {ex.Message}");
            return new Tuple<bool, RequestContext?>(false, null);
        }
    }

    private RequestContext InitialiseRequestContext(UserRecord userRecord, Tenant tenant)
    {
        var result = new RequestContext
        {
            uId = userRecord.Uid,
            tenantId = userRecord.TenantId,
            active = !userRecord.Disabled,
            email = userRecord.Email,
            phoneNumber = userRecord.PhoneNumber,
            displayName = userRecord.DisplayName,
            tenantName = tenant.DisplayName,

            // Set priviledges
            rootPriviledge = tenant.TenantId == GcpConstants.RootTenantId
        };

        return result;
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
                return new AddUserToTenantResponse( Services.Common.ServiceResponseBase.ResponseStatus.Exception, "Error creating user - null returned");
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
        var message = "Unknown Error from Firvase Auth occurred";

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

        return message;
    }
}