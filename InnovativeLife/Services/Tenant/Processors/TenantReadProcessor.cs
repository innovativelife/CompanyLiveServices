using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Localization;

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
    public async Task<TenantReadResponse> Read(IRequestContext requestContext, string tenantId)
    {
        _logger.LogInformation("Executing TenantService Read");

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage("Tenant_Id_Mandatory"));
        }

        var result = await _tenantActions.Read(tenantId);

        if (result.Item1.Success)
        {
            var response = new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Tenant Found")
            {
                tenantId = result.Item2.tenantId,
                tenantName = result.Item2.tenantName
            };
            return response;
        }
        else
        {
            return new TenantReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Tenant not found.  TenantId: {tenantId}");
        }
    }
}