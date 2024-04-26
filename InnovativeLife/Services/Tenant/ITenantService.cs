using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;

namespace InnovativeLife.Services.Tenant;

public interface ITenantService
{
    public Task<TenantAddResponse> Add(RequestContext userContext, TenantAddRequest request);

    public Task<TenantReadResponse> Read(RequestContext userContext, string tenantId);

    public Task<TenantSaveResponse> Save(RequestContext userContext, TenantSaveRequest request);
}