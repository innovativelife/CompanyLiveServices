using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;

namespace InnovativeLife.Services.Tenant;
public class TenantService : ITenantService
{
    private readonly ITenantAddProcessor _tenantAddProcessor;
    private readonly ITenantReadProcessor _tenantReadProcessor;
    private readonly ITenantSaveProcessor _tenantSaveProcessor;

    public TenantService(ITenantAddProcessor tenantAddProcessor, ITenantReadProcessor tenantReadProcessor, ITenantSaveProcessor tenantSaveProcessor)
    {
        _tenantAddProcessor = tenantAddProcessor;
        _tenantReadProcessor = tenantReadProcessor;
        _tenantSaveProcessor = tenantSaveProcessor;
    }

    public async Task<TenantAddResponse> Add(IUserContext requestContext, TenantAddRequest request)
    {
        return await _tenantAddProcessor.Add(requestContext, request);
    }

    public async Task<TenantReadResponse> Read(IUserContext requestContext, string tenantId)
    {
        return await _tenantReadProcessor.ReadSingleton(requestContext, tenantId);
    }

    public async Task<TenantReadSetResponse> ReadSet(IUserContext requestContext)
    {
        return await _tenantReadProcessor.ReadSet(requestContext);
    }

    public async Task<TenantSaveResponse> Save(IUserContext requestContext, string tenantId, TenantSaveRequest request)
    {
        return await _tenantSaveProcessor.Save(requestContext, tenantId, request);
    }
}
