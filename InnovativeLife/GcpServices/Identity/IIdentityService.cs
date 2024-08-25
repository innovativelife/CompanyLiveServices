using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity.ServiceMessages;
using Microsoft.AspNetCore.Authentication;

namespace InnovativeLife.GcpServices.Identity;

public interface IIdentityService
{
    public Task<AuthenticateResult> AuthenticateUserAndTenant(string authToken, string tenantId, IUserContext userContext, string schemeName);

    public Task<AddTenantResponse> AddTenant(string displayName);

    public Task<AddUserToTenantResponse> AddUserToTenant(string tenantId, string displayName, string email, string phoneNumber, string initialPassword);

    public Task<Tuple<bool, string>> SetAdminAuthorisationForUser(string tenantId, string uid, bool adminUser);
}