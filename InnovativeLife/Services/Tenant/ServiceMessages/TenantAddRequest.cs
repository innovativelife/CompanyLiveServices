using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantAddRequest : RequestBase
{
    [Required(ErrorMessage = "Tenant Id must be provided.")]
    public string tenantId { get; set; } = "";
    
    [Required(ErrorMessage = "Tenant Name must be provided.")]
    public string tenantName { get; set; } = "";

    [Required(ErrorMessage = "Customer Name must be provided.")]
    public string customerName { get; set; } = "";

    public EmployeeAddRequest primaryAdministrator { get; set; } = new EmployeeAddRequest();

    public EmployeeAddRequest secondaryAdministrator { get; set; } = new EmployeeAddRequest();

    public DateTime renewalDate { get; set; } = DateTime.SpecifyKind(DateTime.Now.AddYears(1), DateTimeKind.Utc);
    public bool active { get; set; } = true;
}