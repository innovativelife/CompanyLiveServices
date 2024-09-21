using InnovativeLife.Services.Common;

namespace InnovativeLife.GcpServices.Identity.ServiceMessages;

public class ResetUserPasswordResponse : ServiceResponseBase
{
    public ResetUserPasswordResponse(ResponseStatus status, string message) : base(status, message) { }

    public string uId { get; set; } = "";
}
