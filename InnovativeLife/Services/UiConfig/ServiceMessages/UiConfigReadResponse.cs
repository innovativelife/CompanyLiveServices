using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigReadResponse : ServiceResponseBase
{
    public UiConfigReadResponse(ResponseStatus status, string message) : base(status, message) { }

    // public string BackgroundColourHex { get; set; } = "#000000";
    // public string ForegroundColourHex { get; set; } = "#000000";

    public UiConfigItem uiConfig { get; set; }
}