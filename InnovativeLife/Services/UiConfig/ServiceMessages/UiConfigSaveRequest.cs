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

    [Required(ErrorMessage = "highlightColorHex must be provided.")]
    public string highlightColorHex { get; set; } = "";

    [Required(ErrorMessage = "Background colour must be provided.")]
    public string backgroundColorHex { get; set; } = "";

    [Required(ErrorMessage = "homePageSafeAreaColorHex must be provided.")]
    public string homePageSafeAreaColorHex { get; set; } = "";

    [Required(ErrorMessage = "appTitleColorHex must be provided.")]
    public string appTitleColorHex { get; set; } = "";

    [Required(ErrorMessage = "appTitleBackgroundColorHex must be provided.")]
    public string appTitleBackgroundColorHex { get; set; } = "";

}