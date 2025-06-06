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

    [Required(ErrorMessage = "tribesTitle must be provided.")]
    public string tribesTitle { get; set; } = "";

    [Required(ErrorMessage = "moreTitle must be provided.")]
    public string moreTitle { get; set; } = "";

    [Required(ErrorMessage = "titleFontSize must be provided.")]
    public string titleFontSize { get; set; } = "";

    [Required(ErrorMessage = "headingFontSize must be provided.")]
    public string headingFontSize { get; set; } = "";

    [Required(ErrorMessage = "textFontSize must be provided.")]
    public string textFontSize { get; set; } = "";

    [Required(ErrorMessage = "subTextFontSize must be provided.")]
    public string subTextFontSize { get; set; } = "";

    [Required(ErrorMessage = "smallSpacing must be provided.")]
    public string smallSpacing { get; set; } = "";

    [Required(ErrorMessage = "mediumSpacing must be provided.")]
    public string mediumSpacing { get; set; } = "";

    [Required(ErrorMessage = "largeSpacing must be provided.")]
    public string largeSpacing { get; set; } = "";

    [Required(ErrorMessage = "primaryColor must be provided.")]
    public string primaryColor { get; set; } = "";

    [Required(ErrorMessage = "secondaryColor must be provided.")]
    public string secondaryColor { get; set; } = "";

    [Required(ErrorMessage = "tertiaryColor must be provided.")]
    public string tertiaryColor { get; set; } = "";

    [Required(ErrorMessage = "backgroundColor must be provided.")]
    public string backgroundColor { get; set; } = "";

    [Required(ErrorMessage = "textColor must be provided.")]
    public string textColor { get; set; } = "";

    [Required(ErrorMessage = "inputsColor must be provided.")]
    public string inputsColor { get; set; } = "";
}