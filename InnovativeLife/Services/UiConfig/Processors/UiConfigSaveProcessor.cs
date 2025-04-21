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
            headerColorHex = saveRequest.headerColorHex,
            headerLoggedOutColorHex = saveRequest.headerLoggedOutColorHex,
            footerColorHex = saveRequest.footerColorHex,
            footerContentColorHex = saveRequest.footerContentColorHex,
            footerCurrentPageColorHex = saveRequest.footerCurrentPageColorHex,
            cardLightColorHex = saveRequest.cardLightColorHex,
            cardDarkColorHex = saveRequest.cardDarkColorHex,
            buttonColorHex = saveRequest.buttonColorHex,
            buttonTextColorHex = saveRequest.buttonTextColorHex,
            headingColorHex = saveRequest.headingColorHex,
            subHeadingColorHex = saveRequest.subHeadingColorHex,
            backgroundColorHex = saveRequest.backgroundColorHex,
            textColorHex = saveRequest.textColorHex,
            calendarMeetingColorHex = saveRequest.calendarMeetingColorHex,
            calendarTaskColorHex = saveRequest.calendarTaskColorHex,
            calendarEventColorHex = saveRequest.calendarEventColorHex,
            heading1FontSize = saveRequest.heading1FontSize,
            heading2FontSize = saveRequest.heading2FontSize,
            heading3FontSize = saveRequest.heading3FontSize,
            footerFontSize = saveRequest.footerFontSize,
            normalTextFontSize = saveRequest.normalTextFontSize,
            avatarTextFontSize = saveRequest.avatarTextFontSize,
            smallSpacing = saveRequest.smallSpacing,
            mediumSpacing = saveRequest.mediumSpacing,
            largeSpacing = saveRequest.largeSpacing
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
                    uiConfigModel.headerColorHex,
                    uiConfigModel.headerLoggedOutColorHex,
                    uiConfigModel.footerColorHex,
                    uiConfigModel.footerContentColorHex,
                    uiConfigModel.footerCurrentPageColorHex,
                    uiConfigModel.cardLightColorHex,
                    uiConfigModel.cardDarkColorHex,
                    uiConfigModel.buttonColorHex,
                    uiConfigModel.buttonTextColorHex,
                    uiConfigModel.headingColorHex,
                    uiConfigModel.subHeadingColorHex,
                    uiConfigModel.backgroundColorHex,
                    uiConfigModel.textColorHex,
                    uiConfigModel.calendarMeetingColorHex,
                    uiConfigModel.calendarTaskColorHex,
                    uiConfigModel.calendarEventColorHex,
                    uiConfigModel.heading1FontSize,
                    uiConfigModel.heading2FontSize,
                    uiConfigModel.heading3FontSize,
                    uiConfigModel.footerFontSize,
                    uiConfigModel.normalTextFontSize,
                    uiConfigModel.avatarTextFontSize,
                    uiConfigModel.smallSpacing,
                    uiConfigModel.mediumSpacing,
                    uiConfigModel.largeSpacing
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