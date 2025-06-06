using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeSaveRequest : RequestBase
{
    [Required(ErrorMessage = "Employee Email ID must be provided.")]
    [EmailAddress]
    public string email { get; set; } = "";

    [Required(ErrorMessage = "Employee Phone number must be provided.")]
    [Phone]
    public string phoneNumber { get; set; } = "";

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
    public string leaderEmployeeNumber { get; set; } = "";
    public string positionTitle { get; set; } = "";
    public string personalDescription { get; set; } = "";
    public string avatarURL { get; set; } = "";
    public bool active { get; set; } = true;
}