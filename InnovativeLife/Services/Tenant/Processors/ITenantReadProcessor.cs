using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantReadProcessor
{
    public Task<TenantReadResponse> Read(IRequestContext requestContext, string tenantId);
}