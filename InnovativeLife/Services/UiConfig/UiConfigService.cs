using System.Text.Json;
using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.UiConfig;
using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.Services.UiConfig.Processors;

namespace InnovativeLife.Services.UiConfig;

public class UiConfigService : IUiConfigService
{
    private ILogger<UiConfigService> _logger;
    private IUiConfigReadProcessor _readUiConfigProcessor;

    private IUiConfigSaveProcessor _saveUiConfigProcessor;
    // private IUiConfigActions _uiConfigActions;

    public UiConfigService(ILogger<UiConfigService> logger, IUiConfigReadProcessor readUiConfigProcessor, IUiConfigSaveProcessor saveUiConfigProcessor)
    {
        _logger = logger;
        _readUiConfigProcessor = readUiConfigProcessor;
        // _uiConfigActions = uiConfigActions;
        _saveUiConfigProcessor = saveUiConfigProcessor;
    }

    public async Task<UiConfigReadResponse> Read(IUserContext requestContext, string tenantId)
    {
        _logger.LogInformation("UiConfigService.Read: Executing UiConfigService.Read Read");

        return await _readUiConfigProcessor.ReadSingleton(requestContext, tenantId);
    }

    public async Task<UiConfigSaveResponse> Save(IUserContext requestContext, string tenantId, UiConfigSaveRequest configModel)
    {
        _logger.LogInformation("UiConfigService.Save: Executing UiConfigService.Save Save");

        return await _saveUiConfigProcessor.Save(requestContext, tenantId, configModel);
    }
}