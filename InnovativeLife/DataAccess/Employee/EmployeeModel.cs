using Google.Cloud.Firestore;
using InnovativeLife.WebApi;

namespace InnovativeLife.DataAccess.Employee;

[FirestoreData]
public class Employee
{
    [FirestoreProperty]
    public string tenantId { get; set; } = "";
    [FirestoreProperty]
    public string userUID { get; set; } = "";
    [FirestoreProperty]
    public bool tenantAdmin { get; set; }
    [FirestoreProperty]
    public string email { get; set; } = "";
    [FirestoreProperty]
    public string phoneNumber { get; set; } = "";
    [FirestoreProperty]
    public string firstName { get; set; } = "";
    [FirestoreProperty]
    public string lastName { get; set; } = "";
    [FirestoreProperty]
    public string preferredName { get; set; } = "";
    [FirestoreProperty]
    public string employeeNumber { get; set; } = "";
    [FirestoreProperty]
    public string leaderEmployeeNumber { get; set; } = "";
    [FirestoreProperty]
    public string positonTitle { get; set; } = "";
      [FirestoreProperty]
    public string personalDecription { get; set; } = "";
    [FirestoreProperty]
    public bool active { get; set; }
    
    public List<EmployeeGroupMembership> employeeGroupMembershipList { get; set; }
    public List<EmployeeGroupMembership> favoritedEmployeeList { get; set; }
}

[FirestoreData]
public class EmployeeGroupMembership
{
    [FirestoreProperty]
    public string employeeId { get; set; }
    [FirestoreProperty]
    public string employeeGroupMembershipType { get; set; }
}

[FirestoreData]
public class FavoritedEmployeeModel
{
    [FirestoreProperty]
    public string employeeId { get; set; }
}

[FirestoreData]
public class TenantAccessModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string employeeId { get; set; }
    [FirestoreProperty]
    public string employeeGroupMembershipType { get; set; }
    [FirestoreProperty]
    public bool active { get; set; }
}