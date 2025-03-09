using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.UiConfig.ServiceMessages;

public class UiConfigSaveResponse : ServiceResponseBase
{
    public UiConfigSaveResponse(ResponseStatus status, string message) : base(status, message) { }
    public UiConfigSaveResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
    public UiConfigSaveResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }

    public UiConfigItem uiConfigItem { get; set; }
}