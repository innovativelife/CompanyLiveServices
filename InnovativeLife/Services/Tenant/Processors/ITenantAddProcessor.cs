using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantAddProcessor
{
    public Task<TenantAddResponse> Add(RequestContext userContext, TenantAddRequest request);
}