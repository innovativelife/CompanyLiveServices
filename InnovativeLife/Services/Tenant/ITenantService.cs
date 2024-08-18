using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Common;
using Microsoft.AspNetCore.Http;

namespace InnovativeLife.Services.Tenant;

public interface ITenantService
{
    public Task<TenantAddResponse> Add(IRequestContext requestContext, TenantAddRequest request);

    public Task<TenantReadResponse> Read(IRequestContext requestContext, string tenantId);

    public Task<TenantReadSetResponse> ReadSet(IRequestContext requestContext);

    public Task<TenantSaveResponse> Save(IRequestContext requestContext, string tenantId, TenantSaveRequest request);
}