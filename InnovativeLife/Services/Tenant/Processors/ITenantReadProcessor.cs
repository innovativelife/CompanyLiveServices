using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantReadProcessor
{
    public Task<TenantReadResponse> ReadSingleton(IUserContext requestContext, string tenantId);

    public Task<TenantReadSetResponse> ReadSet(IUserContext requestContext);

    public Task<TenantGetIdentityManagerTenantIdResponse> GetIdentityManagerTenantId(IUserContext requestContext, string tenantId);
}