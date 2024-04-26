using InnovativeLife.Services.Common;

namespace InnovativeLife.GcpServices.Identity.ServiceMessages;

public class AddTenantResponse : ServiceResponseBase
{
    public AddTenantResponse(ResponseStatus status, string message) : base(status, message) { }

    public string tenantId { get; set; } = "";
}
