using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.GcpServices.Identity;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantAddProcessor : ITenantAddProcessor
{
    private readonly ILogger<ITenantAddProcessor> _logger;
    private readonly ITenantActions _tenantActions;
    private readonly IIdentityService _identityService;

    public TenantAddProcessor(ILogger<ITenantAddProcessor> logger, ITenantActions tenantActions, IIdentityService identityService)
    {
        _logger = logger;
        _tenantActions = tenantActions;
        _identityService = identityService;
    }
    public async Task<TenantAddResponse> Add(IUserContext requestContext, TenantAddRequest request)
    {
        _logger.LogInformation("Executing TenantService Add");

        var validationResult = request.Validate();
        if (validationResult.Count > 0)
        {
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }

        // Check if Tenant with this Id already exists
        var readByIdResult = await _tenantActions.Read(request.tenantId);

        if (readByIdResult.Item1.Success)
        {
            // Tenant Found in DB
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Duplicate, "Tenant with this ID already exists");
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
            primaryContactName = request.primaryContactName,
            primaryContactEmail = request.primaryContactEmail,
            primaryContactPhone = request.primaryContactPhone,
            secondaryContactName = request.secondaryContactName,
            secondaryContactEmail = request.secondaryContactEmail,
            secondaryContactPhone = request.secondaryContactPhone,
            renewalDate = DateTime.SpecifyKind(request.renewalDate, DateTimeKind.Utc),
            active = true
        };
        var saveResponse = await _tenantActions.Save(tenantModel);

        if (saveResponse.Success)
        {
            var processorResponse = new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Added, "Tenant added succesfully")
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
            return new TenantAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Tenant could not be added due to unexpected DB error");
        }
    }
}