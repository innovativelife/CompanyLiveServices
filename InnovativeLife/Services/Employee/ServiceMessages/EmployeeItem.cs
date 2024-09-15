using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeItem
{
    public EmployeeItem()
    {}
    
    public EmployeeItem(
        string tenantId,
        string employeeUID,
        string email,
        string phoneNumber,
        string firstName,
        string lastName,
        string preferredName,
        string employeeNumber,
        string leaderEmployeeNumber,
        string positonTitle,
        string personalDescription,
        bool active,
        bool adminPrivilege)
    {
        this.tenantId = tenantId;
        this.employeeUID = employeeUID;
        this.email = email;
        this.phoneNumber = phoneNumber;
        this.firstName = firstName;
        this.lastName = lastName;
        this.preferredName = preferredName;
        this.employeeNumber = employeeNumber;
        this.leaderEmployeeNumber = leaderEmployeeNumber;
        this.positionTitle = positonTitle;
        this.personalDescription = personalDescription;
        this.active = active;
        this.adminPrivilege = adminPrivilege;
    }

    public string tenantId { get; set; } = "";

    public string employeeUID { get; set; } = "";

    public string email { get; set; } = "";

    public string phoneNumber { get; set; } = "";

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
    public string leaderEmployeeNumber { get; set; } = "";
    public string positionTitle { get; set; } = "";
    public string personalDescription { get; set; } = "";
    public bool active { get; set; } = true;
    public bool adminPrivilege { get; set; } = true;
}