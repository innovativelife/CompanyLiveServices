using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeAddRequest : ServiceRequestObjectBase
{
    [Required(ErrorMessage = "Tenant Id must be provided.")]
    public string tenantId { get; set; } = "";
    public bool tenantAdmin { get; set; } = false;

    [Required(ErrorMessage = "Employee Email ID must be provided.")]

    [EmailAddress]
    public string email { get; set; } = "";

    [Phone]
    public string phoneNumber { get; set; } = "";
    public string initialPassword { get; set; } = "";

    [Required(ErrorMessage = "First Name must be provided.")]
    public string firstName { get; set; } = "";

    [Required(ErrorMessage = "Last Name must be provided.")]
    public string lastName { get; set; } = "";
    private string _preferredName { get; set; } = "";
    public string preferredName
    {
        get { return string.IsNullOrEmpty(_preferredName) ? firstName : _preferredName; }
        set { _preferredName = value; }
    }
    public string displayName
    {
        get { return preferredName + " " + lastName; }
    }
    public string employeeNumber { get; set; } = "";
    public string leaderEmployeeId { get; set; } = "";
    public string positonTitle { get; set; } = "";
    public string personalDecription { get; set; } = "";
    public bool active { get; set; } = true;
}