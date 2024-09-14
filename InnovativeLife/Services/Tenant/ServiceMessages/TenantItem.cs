using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantItem
{
    public TenantItem() {

    }

    public TenantItem(string tenantId, 
        string tenantName, 
        string identityManagerTenantId, 
        string customerName,
        DateTime renewalDate,
        bool active)
    {
        this.tenantId = tenantId;
        this.tenantName = tenantName;
        this.identityManagerTenantId = identityManagerTenantId;
        this.customerName = customerName;
        this.renewalDate = DateTime.SpecifyKind(renewalDate, DateTimeKind.Utc);
        this.renewalDate = renewalDate;
        this.active = active;
    }

    public string tenantId { get; set; } = "";
    public string identityManagerTenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public string customerName {get; set; } = ""; 
    public EmployeeItem? primaryAdministrator {get; set;}
    public EmployeeItem? secondaryAdministrator {get; set;}
    public DateTime renewalDate {get; set;} = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    public bool active { get; set; } = false;
}