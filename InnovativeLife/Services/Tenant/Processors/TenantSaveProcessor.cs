using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantSaveProcessor : ITenantSaveProcessor
{
    private readonly ILogger<TenantSaveProcessor> _logger;
    private readonly ITenantActions _tenantActions;

    public TenantSaveProcessor(ILogger<TenantSaveProcessor> logger, ITenantActions tenantActions)
    {
        _logger = logger;
        _tenantActions = tenantActions;
    }
    public async Task<TenantSaveResponse> Save(RequestContext userContext, TenantSaveRequest request)
    {
        _logger.LogInformation("Executing TenantService Save");

        if (string.IsNullOrWhiteSpace(request.tenantId))
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Tenant Id cannot be left blank");
        }

        if (string.IsNullOrWhiteSpace(request.tenantName))
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Tenant Name cannot be left blank");
        }

        // Check Tenant already exists - New tenants must be added by "Add"
        var readResult = await _tenantActions.Read(request.tenantId);
        if (!readResult.Item1.Success)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Tenant does not exist.  Use Add action to create a new tenant.");
        }

        // Update tenant
        var tenantModel = new TenantModel
        {
            tenantId = request.tenantId,
            tenantName = request.tenantName,
            active = request.active
        };

        var saveResult = await _tenantActions.Save(tenantModel);

        _logger.LogInformation("TenantService Saved succesfully");
        return new TenantSaveResponse(saveResult.Status, "Tenant saved succesfully");
    }
}