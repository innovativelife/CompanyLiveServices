using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;
using InnovativeLife.Services.Common;
using Microsoft.AspNetCore.Http;

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

    public async Task<TenantAddResponse> Add(IRequestContext requestContext, TenantAddRequest request)
    {
        return await _tenantAddProcessor.Add(requestContext, request);
    }

    public async Task<TenantReadResponse> Read(IRequestContext requestContext, string tenantId)
    {
        return await _tenantReadProcessor.Read(requestContext, tenantId);
    }

    public async Task<TenantSaveResponse> Save(IRequestContext requestContext, TenantSaveRequest request)
    {
        return await _tenantSaveProcessor.Save(requestContext, request);
    }

}
