using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantSaveRequest : ServiceRequestObjectBase
{
    [Required(ErrorMessage = "Tenant Name must be provided.")]
    public string tenantName { get; set; } = "";

    [Required(ErrorMessage = "Customer Name must be provided.")]
    public string customerName {get; set; } = "";
    
    [Required(ErrorMessage = "Primary Contact Name must be provided.")]
    public string primaryContactName {get; set;} = "";
    
    [Required(ErrorMessage = "Primary Contact Email must be provided.")]
    public string primaryContactEmail {get; set;} = "";
    public string primaryContactPhone {get; set;} = "";
    public string secondaryContactName {get; set;} = "";
    public string secondaryContactEmail {get; set;} = "";
    public string secondaryContactPhone {get; set;} = "";
    public DateTime renewalDate {get; set;} = new DateTime();
    public bool active { get; set; }
}