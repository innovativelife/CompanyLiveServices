using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantSaveRequest : ServiceRequestObjectBase
{
    [Required(ErrorMessage = "Tenant Name must be provided.")]
    public string tenantName { get; set; } = "";

    [Required(ErrorMessage = "Customer Name must be provided.")]
    public string customerName {get; set; } = "";

    [Required(ErrorMessage = "Primary Employee UID must be provided.")]
    public string primaryAdministratorEmployeeUID  {get; set; } = "";

    [Required(ErrorMessage = "Secondary Employee UID must be provided.")]
    public string secondaryAdministratorEmployeeUID  {get; set; } = "";    
    public DateTime renewalDate {get; set;} = new DateTime();
    public bool active { get; set; }
}