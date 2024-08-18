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
    public async Task<TenantSaveResponse> Save(IRequestContext requestContext, string tenantId, TenantSaveRequest request)
    {
        _logger.LogInformation("Executing TenantService Save");

        // Root action - tenant must be in root tenancy or must be in dev mode
        if (!requestContext.rootPriviledge && !requestContext.developmentMode)
        {
            _logger.LogCritical("Non root user attempted to add a tenant");
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Unauthorised Add");
        }

        if (string.IsNullOrWhiteSpace(request.tenantName))
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Tenant Name cannot be left blank");
        }

        // Check Tenant already exists - New tenants must be added by "Add"
        var readResult = await _tenantActions.Read(tenantId);
        if (!readResult.Item1.Success)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Tenant does not exist.  Use Add action to create a new tenant.");
        }

        // Update tenant
        var tenantModel = new TenantModel
        {
            tenantId = tenantId,
            tenantName = request.tenantName,
            customerName = request.customerName,
            primaryContactName = request.primaryContactName,
            primaryContactEmail = request.primaryContactEmail,
            primaryContactPhone = request.primaryContactPhone,
            secondaryContactName = request.secondaryContactName,
            secondaryContactEmail = request.secondaryContactEmail,
            secondaryContactPhone = request.secondaryContactPhone,
            renewalDate = DateTime.SpecifyKind(request.renewalDate, DateTimeKind.Utc),
            active = request.active
        };

        var saveResult = await _tenantActions.Save(tenantModel);

        _logger.LogInformation("TenantService Saved succesfully");
        return new TenantSaveResponse(saveResult.Status, "Tenant saved succesfully");
    }
}