using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;

namespace InnovativeLife.Services.Tenant;
public class TenantService : ITenantService
{
    readonly ITenantAddProcessor _tenantAddProcessor;
    readonly ITenantReadProcessor _tenantReadProcessor;
    readonly ITenantSaveProcessor _tenantSaveProcessor;

    public TenantService(ITenantAddProcessor tenantAddProcessor, ITenantReadProcessor tenantReadProcessor, ITenantSaveProcessor tenantSaveProcessor)
    {
        _tenantAddProcessor = tenantAddProcessor;
        _tenantReadProcessor = tenantReadProcessor;
        _tenantSaveProcessor = tenantSaveProcessor;
    }

    public async Task<TenantAddResponse> Add(RequestContext userContext, TenantAddRequest request)
    {
        return await _tenantAddProcessor.Add(userContext, request);
    }

    public async Task<TenantReadResponse> Read(RequestContext userContext, string tenantId)
    {
        return await _tenantReadProcessor.Read(userContext, tenantId);
    }

    public async Task<TenantSaveResponse> Save(RequestContext userContext, TenantSaveRequest request)
    {
        return await _tenantSaveProcessor.Save(userContext, request);
    }

}
