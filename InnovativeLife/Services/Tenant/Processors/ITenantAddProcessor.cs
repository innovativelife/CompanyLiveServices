using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantAddProcessor
{
    public Task<TenantAddResponse> Add(IUserContext requestContext, TenantAddRequest request);
}