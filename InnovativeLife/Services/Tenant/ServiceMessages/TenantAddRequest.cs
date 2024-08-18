using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantAddRequest
{
    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public string customerName {get; set; } = "";
    public string primaryContactName {get; set;} = "";
    public string primaryContactEmail {get; set;} = "";
    public string primaryContactPhone {get; set;} = "";
    public string secondaryContactName {get; set;} = "";
    public string secondaryContactEmail {get; set;} = "";
    public string secondaryContactPhone {get; set;} = "";
    public DateTime renewalDate {get; set;} = DateTime.SpecifyKind(DateTime.Now.AddYears(1), DateTimeKind.Utc);
    public bool active { get; set; } = true;
}