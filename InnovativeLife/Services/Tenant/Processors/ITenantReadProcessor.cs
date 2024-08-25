using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Security;
using Microsoft.AspNetCore.Http;

namespace InnovativeLife.Services.Tenant.Processors;

public interface ITenantReadProcessor
{
    public Task<TenantReadResponse> ReadSingleton(IUserContext requestContext, string tenantId);

    public Task<TenantReadSetResponse> ReadSet(IUserContext requestContext);
}