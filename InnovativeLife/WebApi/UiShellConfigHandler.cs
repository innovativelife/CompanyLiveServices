using Microsoft.Extensions.Logging;
using Google.Cloud.Firestore;
using InnovativeLife.CloudFunctionHandler;
using InnovativeLife.DataAccess.UiShellConfig;
using System.Text.Json;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Common;
using InnovativeLife.Services.UiShellConfig;

namespace InnovativeLife.WebApi;
public class UiShellConfigHandler : IUiShellConfigHandler
{
    ILogger<UiShellConfigHandler> _logger;
    IUiShellConfigService _uiShellConfigService;

    public UiShellConfigHandler(ILogger<UiShellConfigHandler> logger, IUiShellConfigService uiShellConfigService)
    {
        _logger = logger;
        _uiShellConfigService = uiShellConfigService;
    }

    public async Task<WebResponse> ExecuteService(RequestContext userContext, string method, string[] parameters, string body)
    {
        _logger.LogInformation($"Executing Ui Config Service for {method} and with {parameters.Length} parameters");

        if (parameters.Length == 0)
        {
            return StandardResponse.InvalidRequest;
        }

        string[] otherParameters = (parameters.Length > 1) ? parameters[1..] : new string[] { };

        if (parameters[0] == "Read")
        {
            _logger.LogInformation("Performing Read Action");
            return await _uiShellConfigService.Read(userContext, otherParameters[0]);
        }
        else if (parameters[0] == "Save")
        {
            _logger.LogInformation("Performing Save Action");
            UiShellConfigModel? uiShellConfigModel = JsonSerializer.Deserialize<UiShellConfigModel>(body);
            return await _uiShellConfigService.Save(userContext, otherParameters[0], uiShellConfigModel);
        }

        return StandardResponse.InvalidRequest;
    }
}