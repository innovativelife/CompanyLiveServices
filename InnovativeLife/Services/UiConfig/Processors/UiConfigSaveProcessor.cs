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
            searchPromptText = saveRequest.searchPromptText,
            homeSvg = saveRequest.homeSvg,
            peopleSvg = saveRequest.peopleSvg,
            calendarSvg = saveRequest.calendarSvg,
            policySvg = saveRequest.policySvg,
            moreSvg = saveRequest.moreSvg,
            loginTopBarColor = saveRequest.loginTopBarColor,
            loginContainerColor = saveRequest.loginContainerColor,
            loginBackgroundColor = saveRequest.loginBackgroundColor,
            loginButtonsColor = saveRequest.loginButtonsColor,
            loginTextFieldColor = saveRequest.loginTextFieldColor,
            backgroundColor = saveRequest.backgroundColor,
            headingColor = saveRequest.headingColor,
            textColor = saveRequest.textColor,
            textFieldColor = saveRequest.textFieldColor,
            topSearchColor = saveRequest.topSearchColor,
            topSearchBoarderColor = saveRequest.topSearchBoarderColor,
            topBarColor = saveRequest.topBarColor,
            breadCrumbBarColor = saveRequest.breadCrumbBarColor,
            breadCrumbColorRgb = saveRequest.breadCrumbColorRgb,
            bottomBarColor = saveRequest.bottomBarColor,
            widgets1Color = saveRequest.widgets1Color,
            widgets2Color = saveRequest.widgets2Color,
            bottomButttonSelectedColor = saveRequest.bottomButttonSelectedColor,
            bottomButttonUnselectedColor = saveRequest.bottomButttonUnselectedColor,
            buttonColor = saveRequest.buttonColor,
            buttonTextColor = saveRequest.buttonTextColor,
            heading1FontSize = saveRequest.heading1FontSize,
            heading2FontSize = saveRequest.heading2FontSize,
            heading3FontSize = saveRequest.heading3FontSize,
            footerFontSize = saveRequest.footerFontSize,
            normalTextFontSize = saveRequest.normalTextFontSize,
            avatarTextFontSize = saveRequest.avatarTextFontSize,
            smallSpacing = saveRequest.smallSpacing,
            mediumSpacing = saveRequest.mediumSpacing,
            largeSpacing = saveRequest.largeSpacing,
            searchSvg = saveRequest.searchSvg,
            backSvg = saveRequest.backSvg,
            favouriteSvg = saveRequest.favouriteSvg,
            messageSvg = saveRequest.messageSvg,
            phoneCallSvg = saveRequest.phoneCallSvg
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
                    uiConfigModel.searchPromptText,
                    uiConfigModel.homeSvg,
                    uiConfigModel.peopleSvg,
                    uiConfigModel.calendarSvg,
                    uiConfigModel.policySvg,
                    uiConfigModel.moreSvg,
                    uiConfigModel.loginTopBarColor,
                    uiConfigModel.loginContainerColor,
                    uiConfigModel.loginBackgroundColor,
                    uiConfigModel.loginButtonsColor,
                    uiConfigModel.loginTextFieldColor,
                    uiConfigModel.backgroundColor,
                    uiConfigModel.headingColor,
                    uiConfigModel.textColor,
                    uiConfigModel.textFieldColor,
                    uiConfigModel.topSearchColor,
                    uiConfigModel.topSearchBoarderColor,
                    uiConfigModel.topBarColor,
                    uiConfigModel.breadCrumbBarColor,
                    uiConfigModel.breadCrumbColorRgb,
                    uiConfigModel.bottomBarColor,
                    uiConfigModel.widgets1Color,
                    uiConfigModel.widgets2Color,
                    uiConfigModel.bottomButttonSelectedColor,
                    uiConfigModel.bottomButttonUnselectedColor,
                    uiConfigModel.buttonColor,
                    uiConfigModel.buttonTextColor,
                    uiConfigModel.heading1FontSize,
                    uiConfigModel.heading2FontSize,
                    uiConfigModel.heading3FontSize,
                    uiConfigModel.footerFontSize,
                    uiConfigModel.normalTextFontSize,
                    uiConfigModel.avatarTextFontSize,
                    uiConfigModel.smallSpacing,
                    uiConfigModel.mediumSpacing,
                    uiConfigModel.largeSpacing,
                    uiConfigModel.searchSvg,
                    uiConfigModel.backSvg,
                    uiConfigModel.favouriteSvg,
                    uiConfigModel.messageSvg,
                    uiConfigModel.phoneCallSvg
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