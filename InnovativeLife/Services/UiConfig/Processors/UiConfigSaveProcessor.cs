using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.DataAccess.UiConfig;

namespace InnovativeLife.Services.UiConfig.Processors;

public class UiConfigSaveProcessor : IUiConfigSaveProcessor
{
    private readonly ILogger _logger;
    private readonly IUiConfigActions _uiConfigActions;

    public UiConfigSaveProcessor(ILogger<IUiConfigSaveProcessor> logger, IUiConfigActions uiConfigActions)
    {
        _logger = logger;
        _uiConfigActions = uiConfigActions;
    }

    async Task<UiConfigSaveResponse> IUiConfigSaveProcessor.Save(IUserContext requestContext, string tenantId, UiConfigSaveRequest saveRequest)
    {
        _logger.LogInformation("Executing UiConfig Save");

        var validationResult = saveRequest.Validate();
        if (validationResult.Count > 0)
        {
            return new UiConfigSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }

        var uiConfigModel = new UiConfigModel
        {
            tenantId = tenantId,
            configId = saveRequest.configId,
            configName = saveRequest.configName,
            googleFont = saveRequest.googleFont,
            appBannerUrl = saveRequest.appBannerUrl,
            appTitle = saveRequest.appTitle,
            homeTitle = saveRequest.homeTitle,
            peopleTitle = saveRequest.peopleTitle,
            calendarTitle = saveRequest.calendarTitle,
            policyTitle = saveRequest.policyTitle,
            moreTitle = saveRequest.moreTitle,
            primaryColorHex = saveRequest.primaryColorHex,
            highlightColorHex = saveRequest.highlightColorHex,
            backgroundColorHex = saveRequest.backgroundColorHex,
            homePageSafeAreaColorHex = saveRequest.homePageSafeAreaColorHex,
            appTitleColorHex = saveRequest.appTitleColorHex,
            appTitleBackgroundColorHex = saveRequest.appTitleBackgroundColorHex
        };

        var saveResponse = await _uiConfigActions.Save(tenantId, uiConfigModel);


        if (saveResponse.Success)
        {
            _logger.LogInformation("UiConfig Saved successfully");

            var processorResponse = new UiConfigSaveResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "UI Config updated successfully")
            {
                uiConfigItem = new UiConfigItem(
                    uiConfigModel.tenantId,
                    uiConfigModel.configId,
                    uiConfigModel.configName,
                    uiConfigModel.googleFont,
                    uiConfigModel.appBannerUrl,
                    uiConfigModel.appTitle,
                    uiConfigModel.homeTitle,
                    uiConfigModel.peopleTitle,
                    uiConfigModel.calendarTitle,
                    uiConfigModel.policyTitle,
                    uiConfigModel.moreTitle,
                    uiConfigModel.primaryColorHex,
                    uiConfigModel.highlightColorHex,
                    uiConfigModel.backgroundColorHex,
                    uiConfigModel.homePageSafeAreaColorHex,
                    uiConfigModel.appTitleColorHex,
                    uiConfigModel.appTitleBackgroundColorHex
                )
            };
            return processorResponse;
        }
        else
        {
            _logger.LogError($"Error saving Ui Config");
            return new UiConfigSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Ui Config could not be added due to unexpected error");
        }
    }
}