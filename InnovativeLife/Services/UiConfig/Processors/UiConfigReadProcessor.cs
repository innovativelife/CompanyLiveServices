using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.DataAccess.UiConfig;

namespace InnovativeLife.Services.UiConfig.Processors;

public class UiConfigReadProcessor : IUiConfigReadProcessor
{
    private readonly ILogger _logger;
    private readonly IUiConfigActions _uiConfigActions;

    public UiConfigReadProcessor(ILogger<IUiConfigReadProcessor> logger, IUiConfigActions uiConfigActions)
    {
        _logger = logger;
        _uiConfigActions = uiConfigActions;
    }

    public async Task<UiConfigReadResponse> ReadSingleton(IUserContext requestContext, string tenantId)
    {
        _logger.LogInformation("Executing UiConfig Read");

        var result = await _uiConfigActions.Read(tenantId);

        if (result.Item1.Success)
        {
            var response = new UiConfigReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Ui Config for Tenant Found");
            response.uiConfig = getUiConfigItemFromUiConfigModel(result.Item2);

            return response;
        }
        else
        {
            return new UiConfigReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Tenant not found.  TenantId: {tenantId}");
        }


        // Console.WriteLine("Do stuff here Levi");

        // var result = new UiConfigReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Did it");
        // result.BackgroundColourHex = "#FFFFFF";
        // return result;
    }

    private UiConfigItem getUiConfigItemFromUiConfigModel(UiConfigModel uiConfigModel)
    {
        return new UiConfigItem(
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
        );
    }
}