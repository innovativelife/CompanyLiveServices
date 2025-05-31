using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigSaveRequest : RequestBase
{
    [Required(ErrorMessage = "configId must be provided.")]
    public string configId { get; set; } = "";

    [Required(ErrorMessage = "configName must be provided.")]
    public string configName { get; set; } = "";

    [Required(ErrorMessage = "googleFont must be provided.")]
    public string googleFont { get; set; } = "";

    [Required(ErrorMessage = "appBannerUrl must be provided.")]
    public string appBannerUrl { get; set; } = "";

    [Required(ErrorMessage = "appTitle must be provided.")]
    public string appTitle { get; set; } = "";

    [Required(ErrorMessage = "homeTitle must be provided.")]
    public string homeTitle { get; set; } = "";

    [Required(ErrorMessage = "peopleTitle must be provided.")]
    public string peopleTitle { get; set; } = "";

    [Required(ErrorMessage = "calendarTitle must be provided.")]
    public string calendarTitle { get; set; } = "";

    [Required(ErrorMessage = "policyTitle must be provided.")]
    public string policyTitle { get; set; } = "";

    [Required(ErrorMessage = "moreTitle must be provided.")]
    public string moreTitle { get; set; } = "";

    [Required(ErrorMessage = "searchPromptText must be provided.")]
    public string searchPromptText { get; set; } = "";

    [Required(ErrorMessage = "homeSvg must be provided.")]
    public string homeSvg { get; set; } = "";

    [Required(ErrorMessage = "peopleSvg must be provided.")]
    public string peopleSvg { get; set; } = "";

    [Required(ErrorMessage = "calendarSvg must be provided.")]
    public string calendarSvg { get; set; } = "";

    [Required(ErrorMessage = "policySvg must be provided.")]
    public string policySvg { get; set; } = "";

    [Required(ErrorMessage = "moreSvg must be provided.")]
    public string moreSvg { get; set; } = "";

    [Required(ErrorMessage = "loginTopBarColor must be provided.")]
    public string loginTopBarColor { get; set; } = "";

    [Required(ErrorMessage = "loginContainerColor must be provided.")]
    public string loginContainerColor { get; set; } = "";

    [Required(ErrorMessage = "loginBackgroundColor must be provided.")]
    public string loginBackgroundColor { get; set; } = "";

    [Required(ErrorMessage = "loginButtonsColor must be provided.")]
    public string loginButtonsColor { get; set; } = "";

    [Required(ErrorMessage = "loginTextFieldColor must be provided.")]
    public string loginTextFieldColor { get; set; } = "";

    [Required(ErrorMessage = "backgroundColor must be provided.")]
    public string backgroundColor { get; set; } = "";

    [Required(ErrorMessage = "headingColor must be provided.")]
    public string headingColor { get; set; } = "";

    [Required(ErrorMessage = "textColor must be provided.")]
    public string textColor { get; set; } = "";

    [Required(ErrorMessage = "textFieldColor must be provided.")]
    public string textFieldColor { get; set; } = "";

    [Required(ErrorMessage = "topSearchColor must be provided.")]
    public string topSearchColor { get; set; } = "";

    [Required(ErrorMessage = "topSearchBoarderColor must be provided.")]
    public string topSearchBoarderColor { get; set; } = "";

    [Required(ErrorMessage = "topBarColor must be provided.")]
    public string topBarColor { get; set; } = "";

    [Required(ErrorMessage = "breadCrumbBarColor must be provided.")]
    public string breadCrumbBarColor { get; set; } = "";

    [Required(ErrorMessage = "breadCrumbColorRgb must be provided.")]
    public string breadCrumbColorRgb { get; set; } = "";

    [Required(ErrorMessage = "bottomBarColor must be provided.")]
    public string bottomBarColor { get; set; } = "";

    [Required(ErrorMessage = "widgets1Color must be provided.")]
    public string widgets1Color { get; set; } = "";

    [Required(ErrorMessage = "widgets2Color must be provided.")]
    public string widgets2Color { get; set; } = "";

    [Required(ErrorMessage = "bottomButttonSelectedColor must be provided.")]
    public string bottomButttonSelectedColor { get; set; } = "";

    [Required(ErrorMessage = "bottomButttonUnselectedColor must be provided.")]
    public string bottomButttonUnselectedColor { get; set; } = "";

    [Required(ErrorMessage = "buttonColor must be provided.")]
    public string buttonColor { get; set; } = "";

    [Required(ErrorMessage = "buttonTextColor must be provided.")]
    public string buttonTextColor { get; set; } = "";

    [Required(ErrorMessage = "heading1FontSize must be provided.")]
    public string heading1FontSize { get; set; } = "";

    [Required(ErrorMessage = "heading2FontSize must be provided.")]
    public string heading2FontSize { get; set; } = "";

    [Required(ErrorMessage = "heading3FontSize must be provided.")]
    public string heading3FontSize { get; set; } = "";

    [Required(ErrorMessage = "footerFontSize must be provided.")]
    public string footerFontSize { get; set; } = "";

    [Required(ErrorMessage = "normalTextFontSize must be provided.")]
    public string normalTextFontSize { get; set; } = "";

    [Required(ErrorMessage = "avatarTextFontSize must be provided.")]
    public string avatarTextFontSize { get; set; } = "";

    [Required(ErrorMessage = "smallSpacing must be provided.")]
    public string smallSpacing { get; set; } = "";

    [Required(ErrorMessage = "mediumSpacing must be provided.")]
    public string mediumSpacing { get; set; } = "";

    [Required(ErrorMessage = "largeSpacing must be provided.")]
    public string largeSpacing { get; set; } = "";

    [Required(ErrorMessage = "searchSvg must be provided.")]
    public string searchSvg { get; set; } = "";

    [Required(ErrorMessage = "backSvg must be provided.")]
    public string backSvg { get; set; } = "";

    [Required(ErrorMessage = "favouriteSvg must be provided.")]
    public string favouriteSvg { get; set; } = "";

    [Required(ErrorMessage = "messageSvg must be provided.")]
    public string messageSvg { get; set; } = "";

    [Required(ErrorMessage = "phoneCallSvg must be provided.")]
    public string phoneCallSvg { get; set; } = "";
}