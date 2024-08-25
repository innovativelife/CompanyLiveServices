using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;

namespace InnovativeLife.Services.Tenant;

public interface ITenantService
{
    public Task<TenantAddResponse> Add(IUserContext requestContext, TenantAddRequest request);

    public Task<TenantReadResponse> Read(IUserContext requestContext, string tenantId);

    public Task<TenantReadSetResponse> ReadSet(IUserContext requestContext);

    public Task<TenantSaveResponse> Save(IUserContext requestContext, string tenantId, TenantSaveRequest request);
}