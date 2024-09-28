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
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.DataAccess.Employee;

namespace InnovativeLife.GcpServices.Identity;

public class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly ITenantActions _tenantActions;
    private readonly IEmployeeActions _employeeActions;
    public IdentityService(ILogger<IdentityService> logger, ITenantActions tenantActions, IEmployeeActions employeeActions)
    {
        _logger = logger;
        _tenantActions = tenantActions;
        _employeeActions = employeeActions;
    }

    // Use google's Auth API to validate the bearer token, and that the user is valid for tenant
    public async Task<AuthenticateResult> AuthenticateUserAndTenant(string authToken, string tenantId, IUserContext userContext, string schemeName)
    {
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to validate token for tenant {tenantId} with scheme {schemeName}", LogLevel.Information);

        if (FirebaseAuth.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = GcpConstants.ProjectId,
            });
        }

        // Determime Identity Manager Tenant Id
        string identityManagerTenantId;
        if (tenantId == GcpConstants.RootTenantId)
        {
            // Request is for Root tenant
            identityManagerTenantId = GcpConstants.RootIdentityManagerTenantId;
            userContext.customerName = GcpConstants.RootTenantId;
            userContext.tenantId = GcpConstants.RootTenantId;
        }
        else
        {
            // Translate tenant id into the Identity Platform's internal name
            _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to read tenant details for {tenantId} to get identityManagerTenantId");
            var readResult = await _tenantActions.Read(tenantId);
            if (!readResult.Item1.Success)
            {
                return AuthenticateResult.Fail($"IdentityService.AuthenticateUserAndTenant: TenantId {tenantId} not found");
            }

            identityManagerTenantId = readResult.Item2.identityManagerTenantId;
            userContext.customerName = readResult.Item2.customerName;
            userContext.identityManagerTenantId   = readResult.Item2.identityManagerTenantId;
            userContext.tenantId   = tenantId;

            _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Swapped supplied tenant ID {tenantId} for identityManagerTenantId {identityManagerTenantId}");
        }

        // Call Firebase tenant manager to get auth manager 
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to call AuthForTenant API for {identityManagerTenantId}");
        var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(identityManagerTenantId);
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Call to AuthForTenant API succeeded");

        // Validate token using auth manager
        var validateResult = await ValidateToken(authToken, authManager);
        if (!validateResult.Item1)
        {
            return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Token validation failed");
        }
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Verify passed ok");

        // Get user from Identity Manager
        var decodedToken = validateResult.Item2;
        _logger.LogInformation($"IdentityService.GetUserAndTenant: Executing GetUserAndTenant for tenantID: {tenantId}");
        UserRecord userRecord = await authManager.GetUserAsync(decodedToken);

        if (userRecord == null)
        {
            _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Could not get user record from GCP");
            return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Failed to retrieve user from GCP Identity Platform"); ;
        }

        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Retrieved user details - UiD:         {userRecord.Uid}");
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Retrieved user details - TenantId:    {userRecord.TenantId}");

        userContext.uId = userRecord.Uid;

        if (tenantId == GcpConstants.RootTenantId)
        {
            // Super users are in Root Admin, but do not have tenant or employee records in the DB.
            if (!await GetSuperUserDetailsFromIdentityPlatform(tenantId, userRecord, userContext))
            {
                return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Failed to retrieve user or tenant");
            }
        }
        else
        {
            if (!await GetEmployeeDetails(tenantId, userContext))
            {
                return AuthenticateResult.Fail("IdentityService.AuthenticateUserAndTenant: Failed to retrieve user or tenant");
            }
        }

        _logger.LogInformation($"User Id:      {userContext.uId}");
        _logger.LogInformation($"Tenant Id:    {userContext.tenantId}");
        _logger.LogInformation($"Root Tenant?  {userContext.rootAdmin}");
        _logger.LogInformation($"Tenant Admin? {userContext.adminPrivilege}");

        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to get claims from userContext for uId: {userContext.uId} for scheme {schemeName}");
        var claims = AuthorizationPolicies.GetClaims(userContext, _logger);

        var claimsIdentity = new ClaimsIdentity(claims, schemeName);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: Getting ticket");
        var ticket = new AuthenticationTicket(claimsPrincipal, schemeName);

        _logger.LogInformation("Returning success");
        return AuthenticateResult.Success(ticket);
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
    private async Task<bool> GetSuperUserDetailsFromIdentityPlatform(string tenantId, UserRecord userRecord, IUserContext userContext)
    {
        try
        {
            var tenant = await FirebaseAuth.DefaultInstance!.TenantManager.GetTenantAsync(GcpConstants.RootIdentityManagerTenantId);
            _logger.LogInformation($"IdentityService.GetUserAndTenant: Retrieved tenant details - DisplayName: {tenant.DisplayName}");

            userContext.uId = userRecord.Uid;
            userContext.preferredName = userRecord.DisplayName;
            userContext.active = !userRecord.Disabled;
            userContext.email = userRecord.Email;
            userContext.phoneNumber = userRecord.PhoneNumber;
            userContext.tenantId = tenantId;
            userContext.identityManagerTenantId = userRecord.TenantId;
            userContext.customerName = tenant.DisplayName;
            userContext.adminPrivilege = false;
            userContext.rootAdmin = true;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"IdentityService.GetUserAndTenant: Error attempting to read user or tenant record: {ex.Message}");
            return false;
        }
    }

    async Task<bool> GetEmployeeDetails(string tenantId, IUserContext userContext)
    {
        // Get Employee Details from DB
        _logger.LogInformation($"IdentityService.AuthenticateUserAndTenant: About to read employee details for {userContext.uId}");

        var getEmployeeResult = await _employeeActions.ReadByEmployeeUID(tenantId, userContext.uId);
        if (!getEmployeeResult.Item1.Success)
        {
            _logger.LogError($"Failed to retrieve employee details for uid: {userContext.uId}");
            return false;
        }
        userContext.preferredName = getEmployeeResult.Item2!.preferredName;
        userContext.active = getEmployeeResult.Item2.active;
        userContext.email = getEmployeeResult.Item2.email;
        userContext.phoneNumber = getEmployeeResult.Item2.phoneNumber;
        userContext.tenantId = getEmployeeResult.Item2.tenantId;
        userContext.adminPrivilege = getEmployeeResult.Item2.adminPrivilege;
        userContext.rootAdmin = false;
        return true;
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

    public async Task<ResetUserPasswordResponse> ResetUserPassword(string tenantId, string uId, string newPassword, IUserContext requestContext)
    {
        _logger.LogInformation($"IdentityService.ResetUserPassword: resetting password for {requestContext.uId}");

        var authManager = FirebaseAuth.DefaultInstance!.TenantManager.AuthForTenant(requestContext.identityManagerTenantId);

        try
        {
            UserRecordArgs args = new UserRecordArgs()
            {
                Uid = uId,
                Password = newPassword,
            };

            UserRecord userRecord = await authManager.UpdateUserAsync(args);

            // See the UserRecord reference doc for the contents of userRecord.
            Console.WriteLine($"Successfully updated user: {userRecord.Uid}");

            var result = new ResetUserPasswordResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "");
            result.uId = requestContext.uId;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"IdentityService.ResetUserPassword: Error updating password");
            return new ResetUserPasswordResponse(Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, GetErrorMessageFromGcpAuthError(ex));
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