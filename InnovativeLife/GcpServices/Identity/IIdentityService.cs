using InnovativeLife.Common;
using InnovativeLife.GcpServices.Identity.ServiceMessages;

namespace InnovativeLife.GcpServices.Identity;

public interface IIdentityService
{
    public Task<Tuple<bool, RequestContext?>> AuthenticateUserAndTenant(string authToken, string tenantId);

    public Task<AddTenantResponse> AddTenant(string displayName);

    public Task<AddUserToTenantResponse> AddUserToTenant(string tenantId, string displayName, string email, string phoneNumber, string initialPassword);

    public Task<Tuple<bool, string>> SetAdminAuthorisationForUser(string tenantId, string uid, bool adminUser);
}