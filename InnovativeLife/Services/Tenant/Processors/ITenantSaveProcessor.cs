using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantSaveProcessor
{
    public Task<TenantSaveResponse> Save(IUserContext requestContext, string tenantId, TenantSaveRequest request);
}