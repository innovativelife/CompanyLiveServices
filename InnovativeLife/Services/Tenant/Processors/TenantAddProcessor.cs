using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantAddProcessor : ITenantAddProcessor
{
    private readonly ILogger<ITenantAddProcessor> _logger;
    private readonly ITenantActions _tenantActions;
    private readonly IIdentityService _identityService;
    private readonly IEmployeeService _employeeService;

    public TenantAddProcessor(ILogger<ITenantAddProcessor> logger, ITenantActions tenantActions, IIdentityService identityService, IEmployeeService employeeService)
    {
        _logger = logger;
        _tenantActions = tenantActions;
        _identityService = identityService;
        _employeeService = employeeService;
    }
    public async Task<TenantAddResponse> Add(IUserContext requestContext, TenantAddRequest request)
    {
        _logger.LogInformation("Executing TenantService Add");

        var validationResult = request.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }
        validationResult = request.primaryAdministrator.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }
        validationResult = request.secondaryAdministrator.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }

        // Check if Tenant with this Id already exists
        var readByIdResult = await _tenantActions.Read(request.tenantId);

        if (readByIdResult.Item1.Success)
        {
            // Tenant Found in DB
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Duplicate, $"Tenant with TenantId {request.tenantId} already exists");
        }

        // Check if Tenant with this name already exists
        var readByNameResult = await _tenantActions.ReadByName(request.tenantName);
        if (readByNameResult.Item1.Success)
        {
            // Tenant Found in DB
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Duplicate, "Tenant with this name already exists");
        }

        // Add tenant to identity manager
        string identityManagerTenantId;
        if (requestContext.developmentMode)
        {
            _logger.LogInformation("Skipped Identity Service Add action in development mode");
            identityManagerTenantId = "DevMode";
        }
        else
        {
            var addResult = await _identityService.AddTenant(request.tenantName);
            if (!addResult.Success)
            {
                return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, addResult.Message);
            }
            identityManagerTenantId = addResult.tenantId;
        }

        // Add tenant to DB
        var tenantModel = new TenantModel
        {
            tenantId = request.tenantId,
            identityManagerTenantId = identityManagerTenantId,
            tenantName = request.tenantName,
            customerName = request.customerName,
            renewalDate = DateTime.SpecifyKind(request.renewalDate, DateTimeKind.Utc),
            active = true
        };
        var saveResponse = await _tenantActions.Save(tenantModel);

        if (!saveResponse.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Tenant could not be added due to unexpected error");
        }

        // Add Primary Administrator
        var primaryAdminSaveResponse = await _employeeService.Add(requestContext, request.tenantId, request.primaryAdministrator);

        if (!primaryAdminSaveResponse.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, $"Error saving Primary Administrator - {primaryAdminSaveResponse.Message}");
        }
        tenantModel.primaryAdministratorEmployeeUID = primaryAdminSaveResponse.employee.employeeUID;

        // Give Admin Privilege for Primary Adminstrator
        var primaryAdminSetAdminPrivilege = await _employeeService.SetAdminPrivilege(requestContext, request.tenantId, primaryAdminSaveResponse.employee.employeeUID, true);
        if (!primaryAdminSetAdminPrivilege.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, $"Error setting Admin Privilege for Primary Administrator- {primaryAdminSetAdminPrivilege.Message}");
        }

        // Add Secondary Administrator
        var secondaryAdminServiceResponse = await _employeeService.Add(requestContext, request.tenantId, request.secondaryAdministrator);

        if (!secondaryAdminServiceResponse.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, $"Error saving Secondary Administrator - {secondaryAdminServiceResponse.Message}");
        }
        tenantModel.secondaryAdministratorEmployeeUID = secondaryAdminServiceResponse.employee.employeeUID;

        // Give Admin Privilege for Secondary Adminstrator
        var secondaryAdminSetAdminPrivilege = await _employeeService.SetAdminPrivilege(requestContext, request.tenantId, secondaryAdminServiceResponse.employee.employeeUID, true);
        if (!secondaryAdminSetAdminPrivilege.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, $"Error setting Admin Privilege for Secondary Administrator- {secondaryAdminSetAdminPrivilege.Message}");
        }

        // Update the tenant with the new employee ID's
        saveResponse = await _tenantActions.Save(tenantModel);
        if (!saveResponse.Success)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Tenant could not be added due to unexpected error - Could not link Administrators");
        }

        var processorResponse = new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Added, "Tenant added succesfully")
        {
            tenant = new TenantItem
            {
                tenantId = tenantModel.tenantId,
                identityManagerTenantId = tenantModel.identityManagerTenantId,
                tenantName = tenantModel.tenantName,
                customerName = tenantModel.customerName,
                primaryAdministrator = primaryAdminSaveResponse.employee,
                secondaryAdministrator = secondaryAdminServiceResponse.employee,
                renewalDate = tenantModel.renewalDate,
                active = tenantModel.active
            },

        };

        return processorResponse;
    }
}