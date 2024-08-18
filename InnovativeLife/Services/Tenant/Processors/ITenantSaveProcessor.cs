using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantSaveProcessor
{
    public Task<TenantSaveResponse> Save(IRequestContext requestContext, string tenantId, TenantSaveRequest request);
}