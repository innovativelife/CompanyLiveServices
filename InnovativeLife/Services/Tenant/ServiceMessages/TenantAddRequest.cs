using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantAddRequest : ServiceRequestBase
{
    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
}