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

    [Required(ErrorMessage = "primaryColorHex must be provided.")]
    public string primaryColorHex { get; set; } = "";

    [Required(ErrorMessage = "headerColorHex must be provided.")]
    public string headerColorHex { get; set; } = "";

    [Required(ErrorMessage = "headerLoggedOutColorHex must be provided.")]
    public string headerLoggedOutColorHex { get; set; } = "";

    [Required(ErrorMessage = "footerColorHex must be provided.")]
    public string footerColorHex { get; set; } = "";

    [Required(ErrorMessage = "footerContentColorHex must be provided.")]
    public string footerContentColorHex { get; set; } = "";

    [Required(ErrorMessage = "footerCurrentPageColorHex must be provided.")]
    public string footerCurrentPageColorHex { get; set; } = "";

    [Required(ErrorMessage = "cardLightColorHex must be provided.")]
    public string cardLightColorHex { get; set; } = "";

    [Required(ErrorMessage = "cardDarkColorHex must be provided.")]
    public string cardDarkColorHex { get; set; } = "";

    [Required(ErrorMessage = "buttonColorHex must be provided.")]
    public string buttonColorHex { get; set; } = "";

    [Required(ErrorMessage = "buttonTextColorHex must be provided.")]
    public string buttonTextColorHex { get; set; } = "";

    [Required(ErrorMessage = "headingColorHex must be provided.")]
    public string headingColorHex { get; set; } = "";

    [Required(ErrorMessage = "subHeadingColorHex must be provided.")]
    public string subHeadingColorHex { get; set; } = "";

    [Required(ErrorMessage = "backgroundColorHex must be provided.")]
    public string backgroundColorHex { get; set; } = "";

    [Required(ErrorMessage = "textColorHex must be provided.")]
    public string textColorHex { get; set; } = "";

    [Required(ErrorMessage = "calendarMeetingColorHex must be provided.")]
    public string calendarMeetingColorHex { get; set; } = "";

    [Required(ErrorMessage = "calendarTaskColorHex must be provided.")]
    public string calendarTaskColorHex { get; set; } = "";

    [Required(ErrorMessage = "calendarEventColorHex must be provided.")]
    public string calendarEventColorHex { get; set; } = "";

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

}