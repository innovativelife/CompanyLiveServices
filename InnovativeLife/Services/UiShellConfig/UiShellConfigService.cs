using System.Text.Json;
using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Security;
using InnovativeLife.WebApi;

namespace InnovativeLife.Services.UiShellConfig;

public class UiShellConfigService : IUiShellConfigService
{
    private ILogger<UiShellConfigService> _logger;
    private IUiShellConfigActions _uiShellConfigActions;

    public UiShellConfigService(ILogger<UiShellConfigService> logger, IUiShellConfigActions uiShellConfigActions)
    {
        _logger = logger;
        _uiShellConfigActions = uiShellConfigActions;
    }

    public async Task<WebResponse> Read(UserContext requestContext, string configId)
    {
        _logger.LogInformation("UiShellConfigService.Read: Executing GetUIConfig Read");

        try
        {
            var result = await _uiShellConfigActions.Read(configId);
            if (result.Item1.StatusType == WebResponse.StatusTypes.Success)
            {
                return StandardResponse.SuccessWithBody(JsonSerializer.Serialize(result.Item2));
            }
            else
            {
                return result.Item1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"UiShellConfigService.Read: Exception - {ex.Message}");
            return StandardResponse.Error;
        }
    }

    public async Task<WebResponse> Save(UserContext requestContext, string configId, UiShellConfigModel configModel)
    {
        _logger.LogInformation("Executing TenantService Save");

        if (string.IsNullOrWhiteSpace(configId))
        {
            return StandardResponse.InvalidRequest;
        }

        configModel.configId = configId;
        var result= await _uiShellConfigActions.Save(configId, configModel);

        _logger.LogInformation("UiShellConfig Saved");
        return result;
    }
}