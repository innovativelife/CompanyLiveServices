using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Employee;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantSaveProcessor : ITenantSaveProcessor
{
    private readonly ILogger<TenantSaveProcessor> _logger;
    private readonly ITenantActions _tenantActions;
    private readonly IEmployeeService _employeeService;

    public TenantSaveProcessor(ILogger<TenantSaveProcessor> logger, ITenantActions tenantActions, IEmployeeService employeeService)
    {
        _logger = logger;
        _tenantActions = tenantActions;
        _employeeService = employeeService;
    }
    public async Task<TenantSaveResponse> Save(IUserContext requestContext, string tenantId, TenantSaveRequest request)
    {
        _logger.LogInformation("Executing TenantService Save");

        var validationResult = request.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }

        // Root action - tenant must be in root tenancy or must be in dev mode
        if (!requestContext.rootAdmin && !requestContext.developmentMode)
        {
            _logger.LogCritical("Non root user attempted to update a tenant");
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
        
        // Check Primary and Secondary Employees exist
        var primaryEmployeeReadResponse = await _employeeService.ReadByEmployeeUID(requestContext, tenantId, request.primaryAdministratorEmployeeUID);
        if (!primaryEmployeeReadResponse.Success)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Primary Employee with UID { request.primaryAdministratorEmployeeUID} does not exist.");
        }

        var secondaryEmployeeReadResponse = await _employeeService.ReadByEmployeeUID(requestContext, tenantId, request.secondaryAdministratorEmployeeUID);
        if (!secondaryEmployeeReadResponse.Success)
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Secondary Employee with UID { request.secondaryAdministratorEmployeeUID} does not exist.");
        }

        // Update tenant
        var tenantModel = new TenantModel
        {
            tenantId = tenantId,
            identityManagerTenantId = request.identityManagerTenantId,
            tenantName = request.tenantName,
            customerName = request.customerName,
            primaryAdministratorEmployeeUID = request.primaryAdministratorEmployeeUID,
            secondaryAdministratorEmployeeUID = request.secondaryAdministratorEmployeeUID,
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
                    primaryAdministrator = primaryEmployeeReadResponse.employee,
                    secondaryAdministrator = secondaryEmployeeReadResponse.employee,
                    renewalDate = tenantModel.renewalDate,
                    active = tenantModel.active
                }
            };
            return processorResponse;
        }
        else
        {
            return new TenantSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Tenant could not be added due to unexpected error");
        }
    }
}