using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigItem
{
    public UiConfigItem(string tenantId,
       string headingColor,
       string backgroundColor)
    {
        this.tenantId = tenantId;
        this.headingColor = headingColor;
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
        string policyTitle,
        string moreTitle,
        string searchPromptText,
        string homeSvg,
        string peopleSvg,
        string calendarSvg,
        string policySvg,
        string moreSvg,
        string loginTopBarColor,
        string loginContainerColor,
        string loginBackgroundColor,
        string loginButtonsColor,
        string loginTextFieldColor,
        string backgroundColor,
        string headingColor,
        string textColor,
        string textFieldColor,
        string topSearchColor,
        string topSearchBoarderColor,
        string topBarColor,
        string breadCrumbBarColor,
        string breadCrumbColorRgb,
        string bottomBarColor,
        string widgets1Color,
        string widgets2Color,
        string bottomButttonSelectedColor,
        string bottomButttonUnselectedColor,
        string buttonColor,
        string buttonTextColor,
        string heading1FontSize,
        string heading2FontSize,
        string heading3FontSize,
        string footerFontSize,
        string normalTextFontSize,
        string avatarTextFontSize,
        string smallSpacing,
        string mediumSpacing,
        string largeSpacing,
        string searchSvg,
        string backSvg,
        string favouriteSvg,
        string messageSvg,
        string phoneCallSvg,
        string primaryColor,
        string secondaryColor,
        string tertiaryColor,
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
        this.policyTitle = policyTitle;
        this.moreTitle = moreTitle;
        this.searchPromptText = searchPromptText;
        this.homeSvg = homeSvg;
        this.peopleSvg = peopleSvg;
        this.calendarSvg = calendarSvg;
        this.policySvg = policySvg;
        this.moreSvg = moreSvg;
        this.loginTopBarColor = loginTopBarColor;
        this.loginContainerColor = loginContainerColor;
        this.loginBackgroundColor = loginBackgroundColor;
        this.loginButtonsColor = loginButtonsColor;
        this.loginTextFieldColor = loginTextFieldColor;
        this.backgroundColor = backgroundColor;
        this.headingColor = headingColor;
        this.textColor = textColor;
        this.textFieldColor = textFieldColor;
        this.topSearchColor = topSearchColor;
        this.topSearchBoarderColor = topSearchBoarderColor;
        this.topBarColor = topBarColor;
        this.breadCrumbBarColor = breadCrumbBarColor;
        this.breadCrumbColorRgb = breadCrumbColorRgb;
        this.bottomBarColor = bottomBarColor;
        this.widgets1Color = widgets1Color;
        this.widgets2Color = widgets2Color;
        this.bottomButttonSelectedColor = bottomButttonSelectedColor;
        this.bottomButttonUnselectedColor = bottomButttonUnselectedColor;
        this.buttonColor = buttonColor;
        this.buttonTextColor = buttonTextColor;
        this.heading1FontSize = heading1FontSize;
        this.heading2FontSize = heading2FontSize;
        this.heading3FontSize = heading3FontSize;
        this.footerFontSize = footerFontSize;
        this.normalTextFontSize = normalTextFontSize;
        this.avatarTextFontSize = avatarTextFontSize;
        this.smallSpacing = smallSpacing;
        this.mediumSpacing = mediumSpacing;
        this.largeSpacing = largeSpacing;
        this.searchSvg = searchSvg;
        this.backSvg = backSvg;
        this.favouriteSvg = favouriteSvg;
        this.messageSvg = messageSvg;
        this.phoneCallSvg = phoneCallSvg;
        this.primaryColor = primaryColor;
        this.secondaryColor = secondaryColor;
        this.tertiaryColor = tertiaryColor;
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
    public string policyTitle { get; set; }
    public string moreTitle { get; set; }
    public string searchPromptText { get; set; }
    public string homeSvg { get; set; }
    public string peopleSvg { get; set; }
    public string calendarSvg { get; set; }
    public string policySvg { get; set; }
    public string moreSvg { get; set; }
    // public string primaryColorHex { get; set; }
    // public string highlightColorHex { get; set; }
    // public string backgroundColorHex { get; set; }
    // public string homePageSafeAreaColorHex { get; set; }
    // public string appTitleColorHex { get; set; }
    // public string appTitleBackgroundColorHex { get; set; }

    public string loginTopBarColor { get; set; }
    public string loginContainerColor { get; set; }
    public string loginBackgroundColor { get; set; }
    public string loginButtonsColor { get; set; }
    public string loginTextFieldColor { get; set; }
    public string backgroundColor { get; set; }
    public string headingColor { get; set; }
    public string textColor { get; set; }
    public string textFieldColor { get; set; }
    public string topSearchColor { get; set; }
    public string topSearchBoarderColor { get; set; }
    public string topBarColor { get; set; }
    public string breadCrumbBarColor { get; set; }
    public string breadCrumbColorRgb { get; set; }
    public string bottomBarColor { get; set; }
    public string widgets1Color { get; set; }
    public string widgets2Color { get; set; }
    public string bottomButttonSelectedColor { get; set; }
    public string bottomButttonUnselectedColor { get; set; }
    public string buttonColor { get; set; }
    public string buttonTextColor { get; set; }
    public string heading1FontSize { get; set; }
    public string heading2FontSize { get; set; }
    public string heading3FontSize { get; set; }
    public string footerFontSize { get; set; }
    public string normalTextFontSize { get; set; }
    public string avatarTextFontSize { get; set; }
    public string smallSpacing { get; set; }
    public string mediumSpacing { get; set; }
    public string largeSpacing { get; set; }
    public string searchSvg { get; set; }
    public string backSvg { get; set; }
    public string favouriteSvg { get; set; }
    public string messageSvg { get; set; }
    public string phoneCallSvg { get; set; }
    public string primaryColor { get; set; }
    public string secondaryColor { get; set; }
    public string tertiaryColor { get; set; }
    public string inputsColor { get; set; }
}

