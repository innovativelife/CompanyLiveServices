using Microsoft.Extensions.Logging;
using System.Text.Json;
using InnovativeLife.Common;
using InnovativeLife.Services.Tenant;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Services.Tenant.ServiceMessages;
using System.Net.WebSockets;

namespace InnovativeLife.WebApi;
public class TenantHandler : ITenantHandler
{
    readonly ILogger<ITenantHandler> _logger;
    readonly ITenantService _tenantService;

    public TenantHandler(ILogger<ITenantHandler> logger, ITenantService tenantService)
    {
        _logger = logger;
        _tenantService = tenantService;
    }

    public async Task<WebResponse> ExecuteService(RequestContext userContext, string method, string[] parameters, string body)
    {
        _logger.LogInformation($"Executing TenantService for {method} and with {parameters.Length} parameters");

        if (parameters.Length == 0)
        {
            return StandardResponse.InvalidRequest;
        }

        switch (parameters[0])
        {
            case "Add":
                return await AddTenant(userContext, body);
            case "Read":
                return await ReadTenant(userContext, parameters);
            case "Save":
                return await SaveTenant(userContext, body);
            default:
                return StandardResponse.InvalidRequest;
        }
    }

    private async Task<WebResponse> AddTenant(RequestContext userContext, string body)
    {
        _logger.LogInformation("Performing Add Action");
        TenantAddRequest? tenantAddRequest;
        try
        {
            tenantAddRequest = JsonSerializer.Deserialize<TenantAddRequest>(body);
        }
        catch (Exception)
        {
            return new WebResponse(TenantAddResponse.ResponseStatus.BadRequest, "Invalid Tenant Data");
        }

        _logger.LogInformation("Performing Add Tenant Service");
        var saveResponse = await _tenantService.Add(userContext, tenantAddRequest);
        return new WebResponse(saveResponse.Status, JsonSerializer.Serialize(saveResponse));
    }

    private async Task<WebResponse> ReadTenant(RequestContext userContext, string[] parameters)
    {
        _logger.LogInformation("Performing Read Action");
        var tenantReadResponse = await _tenantService.Read(userContext, parameters[1]);
        return new WebResponse(tenantReadResponse.Status, JsonSerializer.Serialize(tenantReadResponse));
    }

    private async Task<WebResponse> SaveTenant(RequestContext userContext, string body)
    {
        _logger.LogInformation("Performing Save Action");
        TenantSaveRequest? tenantSaveRequest = JsonSerializer.Deserialize<TenantSaveRequest>(body);
        var saveResponse = await _tenantService.Save(userContext, tenantSaveRequest);
        return new WebResponse(saveResponse.Status, JsonSerializer.Serialize(saveResponse));
    }
}