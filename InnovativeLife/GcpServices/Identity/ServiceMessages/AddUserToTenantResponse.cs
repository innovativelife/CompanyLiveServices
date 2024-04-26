using InnovativeLife.Services.Common;

namespace InnovativeLife.GcpServices.Identity.ServiceMessages;

public class AddUserToTenantResponse : ServiceResponseBase
{
    public AddUserToTenantResponse(ResponseStatus status, string message) : base(status, message) { }

    public string uId { get; set; } = "";
}