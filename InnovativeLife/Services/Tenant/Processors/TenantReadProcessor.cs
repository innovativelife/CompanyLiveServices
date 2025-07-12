using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Localization;
using InnovativeLife.Services.Employee;
using InnovativeLife.Services.Employee.ServiceMessages;
using Microsoft.AspNetCore.Http;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantReadProcessor : ITenantReadProcessor
{
    private readonly ILogger<TenantAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly ITenantActions _tenantActions;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IEmployeeService _employeeService;

    public TenantReadProcessor(ILogger<TenantAddProcessor> logger, IMessageService messageService, ITenantActions tenantActions, IHttpContextAccessor httpContext, IEmployeeService employeeService)
    {
        _logger = logger;
        _messageService = messageService;
        _tenantActions = tenantActions;
        _httpContext = httpContext;
        _employeeService = employeeService;
    }
    public async Task<TenantReadResponse> ReadSingleton(IUserContext requestContext, string tenantId)
    {
        _logger.LogInformation("Executing TenantService Read");

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage("Tenant_Id_Mandatory"));
        }

        var result = await _tenantActions.Read(tenantId);

        if (result.Item1.Success)
        {
            var response = new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant Found");
            response.tenant = getTenantItemFromTenantModel(result.Item2);
            await readPrimaryAndSecondaryEmployees(requestContext, response.tenant, result.Item2);

            return response;
        }
        else
        {
            return new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Tenant not found.  TenantId: {tenantId}");
        }
    }

    public async Task<TenantReadSetResponse> ReadSet(IUserContext requestContext)
    {
        _logger.LogInformation("Executing TenantService Read");

        var result = await _tenantActions.ReadSet();

        if (result.Item1.Success && result.Item2.Count > 0)
        {
            var tenantItems = new List<TenantItem>();
            foreach (var tenant in result.Item2)
            {
                var tenantItem = getTenantItemFromTenantModel(tenant);
                tenantItems.Add(tenantItem);

                await readPrimaryAndSecondaryEmployees(requestContext, tenantItem, tenant);
            }
            var response = new TenantReadSetResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"{tenantItems.Count} Tenants Found", tenantItems);
            return response;
        }
        else
        {
            return new TenantReadSetResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "No tenants found", new List<TenantItem>());
        }
    }

    public async Task<TenantGetIdentityManagerTenantIdResponse> GetIdentityManagerTenantId(IUserContext requestContext, string tenantId)
    {
        _logger.LogInformation("Executing TenantService GetGcpTenantId");

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new TenantGetIdentityManagerTenantIdResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage("Tenant_Id_Mandatory"));
        }

        var result = await _tenantActions.Read(tenantId);

        if (result.Item1.Success)
        {
            var response = new TenantGetIdentityManagerTenantIdResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant Found");
            response.identityManagerTenantIdTenantId = getTenantItemFromTenantModel(result.Item2!).identityManagerTenantId;

            return response;
        }
        else
        {
            return new TenantGetIdentityManagerTenantIdResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Tenant not found.  TenantId: {tenantId}");
        }
    }

    private async Task readPrimaryAndSecondaryEmployees(IUserContext requestContext, TenantItem tenantItem, TenantModel tenant)
    {
        var primaryEmployeeReadResponse = await _employeeService.ReadByEmployeeUID(requestContext, tenant.tenantId, tenant.primaryAdministratorEmployeeUID, true);
        if (primaryEmployeeReadResponse.Success)
        {
            tenantItem.primaryAdministrator = primaryEmployeeReadResponse.employee;
        }

        var secondaryEmployeeReadResponse = await _employeeService.ReadByEmployeeUID(requestContext, tenant.tenantId, tenant.secondaryAdministratorEmployeeUID, true);
        if (secondaryEmployeeReadResponse.Success)
        {
            tenantItem.secondaryAdministrator = secondaryEmployeeReadResponse.employee;
        }
    }

    private TenantItem getTenantItemFromTenantModel(TenantModel tenantModel)
    {
        return new TenantItem(
            tenantModel.tenantId,
            tenantModel.tenantName,
            tenantModel.identityManagerTenantId,
            tenantModel.customerName,
            tenantModel.renewalDate,
            tenantModel.active
        );
    }

}