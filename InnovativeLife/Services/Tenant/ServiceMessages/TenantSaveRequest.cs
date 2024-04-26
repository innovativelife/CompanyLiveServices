namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantSaveRequest
{
    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public bool active { get; set; }
}