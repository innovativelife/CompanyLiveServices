using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
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
    public async Task<TenantSaveResponse> Save(IUserContext requestContext, string tenantId, TenantSaveRequest request)
    {
        _logger.LogInformation("Executing TenantService Save");

        var validationResult = request.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, validationResult);
        }

        // Root action - tenant must be in root tenancy or must be in dev mode
        if (!requestContext.rootAdmin && !requestContext.developmentMode)
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
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "Tenant does not exist.  Use Add action to create a new tenant.");
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

        var saveResponse = await _tenantActions.Save(tenantModel);

        if (saveResponse.Success)
        {
            _logger.LogInformation("TenantService Saved successfully");

            var processorResponse = new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant updated successfully")
            {
                tenant = new TenantItem
                {
                    tenantId = tenantModel.tenantId,
                    identityManagerTenantId = tenantModel.identityManagerTenantId,
                    tenantName = tenantModel.tenantName,
                    customerName = tenantModel.customerName,
                    primaryContactName = tenantModel.primaryContactName,
                    primaryContactPhone = tenantModel.primaryContactPhone,
                    secondaryContactName = tenantModel.secondaryContactName,
                    secondaryContactEmail = tenantModel.secondaryContactEmail,
                    renewalDate = tenantModel.renewalDate,
                    active = tenantModel.active
                }
            };
            return processorResponse;
        }
        else
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Tenant could not be added due to unexpected DB error");
        }
    }
}