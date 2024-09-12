using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantReadResponse : ServiceResponseBase
{
    public TenantReadResponse(ResponseStatus status, string message) : base(status, message) { }

    public TenantItem tenant { get; set; }
}