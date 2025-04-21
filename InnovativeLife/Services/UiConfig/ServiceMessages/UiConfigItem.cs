using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigItem
{
    // public UiConfigItem(string tenantId,
    //    string primaryColorHex,
    //    string backgroundColorHex)
    // {
    //     this.tenantId = tenantId;
    //     this.primaryColorHex = primaryColorHex;
    //     this.backgroundColorHex = backgroundColorHex;
    // }

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
        string headerColorHex,
        string headerLoggedOutColorHex,
        string footerColorHex,
        string footerContentColorHex,
        string footerCurrentPageColorHex,
        string cardLightColorHex,
        string cardDarkColorHex,
        string buttonColorHex,
        string buttonTextColorHex,
        string headingColorHex,
        string subHeadingColorHex,
        string backgroundColorHex,
        string textColorHex,
        string calendarMeetingColorHex,
        string calendarTaskColorHex,
        string calendarEventColorHex,
        string heading1FontSize,
        string heading2FontSize,
        string heading3FontSize,
        string footerFontSize,
        string normalTextFontSize,
        string avatarTextFontSize,
        string smallSpacing,
        string mediumSpacing,
        string largeSpacing)
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
        this.headerColorHex = headerColorHex;
        this.headerLoggedOutColorHex = headerLoggedOutColorHex;
        this.footerColorHex = footerColorHex;
        this.footerContentColorHex = footerContentColorHex;
        this.footerCurrentPageColorHex = footerCurrentPageColorHex;
        this.cardLightColorHex = cardLightColorHex;
        this.cardDarkColorHex = cardDarkColorHex;
        this.buttonColorHex = buttonColorHex;
        this.buttonTextColorHex = buttonTextColorHex;
        this.headingColorHex = headingColorHex;
        this.subHeadingColorHex = subHeadingColorHex;
        this.backgroundColorHex = backgroundColorHex;
        this.textColorHex = textColorHex;
        this.calendarMeetingColorHex = calendarMeetingColorHex;
        this.calendarTaskColorHex = calendarTaskColorHex;
        this.calendarEventColorHex = calendarEventColorHex;
        this.heading1FontSize = heading1FontSize;
        this.heading2FontSize = heading2FontSize;
        this.heading3FontSize = heading3FontSize;
        this.footerFontSize = footerFontSize;
        this.normalTextFontSize = normalTextFontSize;
        this.avatarTextFontSize = avatarTextFontSize;
        this.smallSpacing = smallSpacing;
        this.mediumSpacing = mediumSpacing;
        this.largeSpacing = largeSpacing;
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
    public string headerColorHex { get; set; }
    public string headerLoggedOutColorHex { get; set; }
    public string footerColorHex { get; set; }
    public string footerContentColorHex { get; set; }
    public string footerCurrentPageColorHex { get; set; }
    public string cardLightColorHex { get; set; }
    public string cardDarkColorHex { get; set; }
    public string buttonColorHex { get; set; }
    public string buttonTextColorHex { get; set; }
    public string headingColorHex { get; set; }
    public string subHeadingColorHex { get; set; }
    public string backgroundColorHex { get; set; }
    public string textColorHex { get; set; }
    public string calendarMeetingColorHex { get; set; }
    public string calendarTaskColorHex { get; set; }
    public string calendarEventColorHex { get; set; }
    public string heading1FontSize { get; set; }
    public string heading2FontSize { get; set; }
    public string heading3FontSize { get; set; }
    public string footerFontSize { get; set; }
    public string normalTextFontSize { get; set; }
    public string avatarTextFontSize { get; set; }
    public string smallSpacing { get; set; }
    public string mediumSpacing { get; set; }
    public string largeSpacing { get; set; }
}

