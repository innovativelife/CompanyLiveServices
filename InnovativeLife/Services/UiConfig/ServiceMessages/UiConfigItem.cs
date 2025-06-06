using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigItem
{
    public UiConfigItem(string tenantId,
       string backgroundColor)
    {
        this.tenantId = tenantId;
        this.backgroundColor = backgroundColor;
    }

    public UiConfigItem(
        string tenantId,
        string configId,
        string configName,
        string googleFont,
        string appBannerUrl,
        string appTitle,
        string homeTitle,
        string peopleTitle,
        string calendarTitle,
        string tribesTitle,
        string moreTitle,
        string titleFontSize,
        string headingFontSize,
        string textFontSize,
        string subTextFontSize,
        string smallSpacing,
        string mediumSpacing,
        string largeSpacing,
        string primaryColor,
        string secondaryColor,
        string tertiaryColor,
        string backgroundColor,
        string textColor,
        string inputsColor
    )
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
        this.tribesTitle = tribesTitle;
        this.moreTitle = moreTitle;
        this.titleFontSize = titleFontSize;
        this.headingFontSize = headingFontSize;
        this.textFontSize = textFontSize;
        this.subTextFontSize = subTextFontSize;
        this.smallSpacing = smallSpacing;
        this.mediumSpacing = mediumSpacing;
        this.largeSpacing = largeSpacing;
        this.primaryColor = primaryColor;
        this.secondaryColor = secondaryColor;
        this.tertiaryColor = tertiaryColor;
        this.backgroundColor = backgroundColor;
        this.textColor = textColor;
        this.inputsColor = inputsColor;
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
    public string tribesTitle { get; set; }
    public string moreTitle { get; set; }
    public string titleFontSize { get; set; }
    public string headingFontSize { get; set; }
    public string textFontSize { get; set; }
    public string subTextFontSize { get; set; }
    public string smallSpacing { get; set; }
    public string mediumSpacing { get; set; }
    public string largeSpacing { get; set; }
    public string primaryColor { get; set; }
    public string secondaryColor { get; set; }
    public string tertiaryColor { get; set; }
    public string backgroundColor { get; set; }
    public string textColor { get; set; }
    public string inputsColor { get; set; }
}

