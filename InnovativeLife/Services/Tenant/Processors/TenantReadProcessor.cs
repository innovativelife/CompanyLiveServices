using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Localization;
using Google.Protobuf;

namespace InnovativeLife.Services.Tenant.Processors;

public class TenantReadProcessor : ITenantReadProcessor
{
    private readonly ILogger<TenantAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly ITenantActions _tenantActions;

    public TenantReadProcessor(ILogger<TenantAddProcessor> logger, IMessageService messageService, ITenantActions tenantActions)
    {
        _logger = logger;
        _messageService = messageService;
        _tenantActions = tenantActions;
    }
    public async Task<TenantReadResponse> ReadSingleton(IRequestContext requestContext, string tenantId)
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
            return response;
        }
        else
        {
            return new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Tenant not found.  TenantId: {tenantId}");
        }
    }

    public async Task<TenantReadSetResponse> ReadSet(IRequestContext requestContext)
    {
        _logger.LogInformation("Executing TenantService Read");

        var result = await _tenantActions.ReadSet();

        if (result.Item1.Success && result.Item2.Count > 0)
        {
            var tenantItems = new List<TenantItem>();
            foreach (var item in result.Item2)
            {
                tenantItems.Add(getTenantItemFromTenantModel(item));
            }
            var response = new TenantReadSetResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"{tenantItems.Count} Tenants Found", tenantItems);
            return response;
        }
        else
        {
            return new TenantReadSetResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "No tenants found", new List<TenantItem>());
        }
    }

    private TenantItem getTenantItemFromTenantModel(TenantModel tenantModel)
    {
        return new TenantItem(
            tenantModel.tenantId,
            tenantModel.tenantName,
            tenantModel.identityManagerTenantId,
            tenantModel.customerName,
            tenantModel.primaryContactName,
            tenantModel.primaryContactEmail,
            tenantModel.primaryContactPhone,
            tenantModel.secondaryContactName,
            tenantModel.secondaryContactEmail,
            tenantModel.secondaryContactPhone,
            tenantModel.renewalDate,
            tenantModel.active
        );
    }
}