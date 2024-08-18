using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantReadProcessor
{
    public Task<TenantReadResponse> ReadSingleton(IRequestContext requestContext, string tenantId);

    public Task<TenantReadSetResponse> ReadSet(IRequestContext requestContext);
}