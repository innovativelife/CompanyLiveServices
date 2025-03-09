using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigItem
{
    public UiConfigItem(string tenantId,
       string primaryColorHex,
       string backgroundColorHex)
    {
        this.tenantId = tenantId;
        this.primaryColorHex = primaryColorHex;
        this.backgroundColorHex = backgroundColorHex;
    }

    public UiConfigItem(string tenantId,
        string configId,
        string configName,
        string googleFont,
        string appBannerUrl,
        string appTitle,
        string homeTitle,
        string peopleTitle,
        string calendarTitle,
        string policyTitle,
        string moreTitle,
        string primaryColorHex,
        string highlightColorHex,
        string backgroundColorHex,
        string homePageSafeAreaColorHex,
        string appTitleColorHex,
        string appTitleBackgroundColorHex)
    {
        this.tenantId = tenantId;
        this.configId = configId;
        this.configName = configName;
        this.googleFont = googleFont;
        this.appBannerUrl = appBannerUrl;
        this.appTitle = appTitle;
        this.homeTitle = homeTitle;
        this.peopleTitle = peopleTitle;
        this.calendarTitle = calendarTitle;
        this.policyTitle = policyTitle;
        this.moreTitle = moreTitle;
        this.primaryColorHex = primaryColorHex;
        this.highlightColorHex = highlightColorHex;
        this.backgroundColorHex = backgroundColorHex;
        this.homePageSafeAreaColorHex = homePageSafeAreaColorHex;
        this.appTitleColorHex = appTitleColorHex;
        this.appTitleBackgroundColorHex = appTitleBackgroundColorHex;
    }
    public string tenantId { get; set; }
    public string configId { get; set; }
    public string configName { get; set; }
    public string googleFont { get; set; }
    public string appBannerUrl { get; set; }
    public string appTitle { get; set; }
    public string homeTitle { get; set; }
    public string peopleTitle { get; set; }
    public string calendarTitle { get; set; }
    public string policyTitle { get; set; }
    public string moreTitle { get; set; }
    public string primaryColorHex { get; set; }
    public string highlightColorHex { get; set; }
    public string backgroundColorHex { get; set; }
    public string homePageSafeAreaColorHex { get; set; }
    public string appTitleColorHex { get; set; }
    public string appTitleBackgroundColorHex { get; set; }
}

