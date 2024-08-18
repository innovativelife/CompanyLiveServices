using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantReadSetResponse : ServiceResponseBase
{
    public TenantReadSetResponse(ResponseStatus status, string message, List<TenantItem>? tenants) : base(status, message) 
    { 
        this.tenants = tenants;
    }

    public List<TenantItem> tenants { get; set; } = new List<TenantItem>();
}