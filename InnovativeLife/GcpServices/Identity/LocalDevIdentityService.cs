using InnovativeLife.Security;
using Microsoft.Extensions.Logging;
using InnovativeLife.GcpServices.Identity.ServiceMessages;
using Microsoft.AspNetCore.Authentication;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.DataAccess.Employee;

namespace InnovativeLife.GcpServices.Identity;

public class LocalDevIdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    public LocalDevIdentityService(ILogger<IdentityService> logger)
    {
        _logger = logger;
    }


    public Task<AuthenticateResult> AuthenticateUserAndTenant(string authToken, string tenantId, IUserContext userContext, string schemeName)
    {
        // Not used for Local Development
        throw new NotImplementedException();
    }

    public async Task<AddTenantResponse> AddTenant(string displayName)
    {
        _logger.LogInformation($"IdentityServiceLocalDev.AddTenant: Skipping add of tenant to GCP {displayName} in dev mode");

        return new AddTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Dev Mode - Skipped add of tenant to GCP");
    }

    public async Task<AddUserToTenantResponse> AddUserToTenant(string tenantId, string displayName, string email, string phoneNumber, string initialPassword, IUserContext requestContext)
    {
        _logger.LogInformation($"IdentityServiceLocalDev.AddUserToTenant: Skipping adding of user {displayName} to tenant {tenantId} in dev mode");

        var response = new AddUserToTenantResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Dev Mode - Skipped add of user to tenant for GCP");
        response.uId = Guid.NewGuid().ToString();

        return response;
    }

    public async Task<ResetUserPasswordResponse> ResetUserPassword(string tenantId, string uId, string newPassword, IUserContext requestContext)
    {
        _logger.LogInformation($"IdentityServiceLocalDev.ResetUserPassword: Skipping reset of user {requestContext.uId} password in dev mode");

        var result = new ResetUserPasswordResponse(Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Dev Mode - Skipped password reset");
        result.uId = requestContext.uId;
        return result;
    }
}