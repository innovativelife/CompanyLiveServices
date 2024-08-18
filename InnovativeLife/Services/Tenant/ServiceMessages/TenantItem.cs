using System;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantItem
{
    public TenantItem() {

    }

    public TenantItem(string tenantId, 
        string tenantName, 
        string identityManagerTenantId, 
        string customerName,
        string primaryContactName,
        string primaryContactEmail,
        string primaryContactPhone,
        string secondaryContactName,
        string secondaryContactEmail,
        string secondaryContactPhone,
        DateTime renewalDate,
        bool active)
    {
        this.tenantId = tenantId;
        this.tenantName = tenantName;
        this.identityManagerTenantId = identityManagerTenantId;
        this.customerName = customerName;
        this.primaryContactName = primaryContactName;
        this.primaryContactEmail = primaryContactEmail;
        this.primaryContactPhone = primaryContactPhone;
        this.secondaryContactName = secondaryContactName;
        this.secondaryContactEmail = secondaryContactEmail;
        this.secondaryContactPhone = secondaryContactPhone;
        this.renewalDate = DateTime.SpecifyKind(renewalDate, DateTimeKind.Utc);
        this.renewalDate = renewalDate;
        this.active = active;
    }

    public string tenantId { get; set; } = "";
    public string identityManagerTenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public string customerName {get; set; } = "";
    public string primaryContactName {get; set;} = "";
    public string primaryContactEmail {get; set;} = "";
    public string primaryContactPhone {get; set;} = "";
    public string secondaryContactName {get; set;} = "";
    public string secondaryContactEmail {get; set;} = "";
    public string secondaryContactPhone {get; set;} = "";
    public DateTime renewalDate {get; set;} = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    public bool active { get; set; } = false;
}