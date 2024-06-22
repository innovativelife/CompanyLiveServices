using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeAddRequest : ServiceRequestBase
{
    public string tenantId { get; set; } = "";
    public bool tenantAdmin { get; set; } = false;
    public string email { get; set; } = "";
    public string phoneNumber { get; set; } = "";
    public string initialPassword { get; set; } = "";
    public string firstName { get; set; } = "";
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