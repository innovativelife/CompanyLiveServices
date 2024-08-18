namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantSaveRequest
{
    public string tenantName { get; set; } = "";
    public string customerName {get; set; } = "";
    public string primaryContactName {get; set;} = "";
    public string primaryContactEmail {get; set;} = "";
    public string primaryContactPhone {get; set;} = "";
    public string secondaryContactName {get; set;} = "";
    public string secondaryContactEmail {get; set;} = "";
    public string secondaryContactPhone {get; set;} = "";
    public DateTime renewalDate {get; set;} = new DateTime();
    public bool active { get; set; }
}