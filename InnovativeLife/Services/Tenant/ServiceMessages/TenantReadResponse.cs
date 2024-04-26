using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantReadResponse : ServiceResponseBase
{
    public TenantReadResponse(ResponseStatus status, string message) : base(status, message) { }

    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public bool active { get; set; } = false;
}