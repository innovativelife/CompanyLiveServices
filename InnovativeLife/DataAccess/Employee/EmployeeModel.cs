using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.Employee;

[FirestoreData]
public class EmployeeModel
{
    [FirestoreProperty]
    public string employeeId { get; set; }
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public bool active { get; set; }
    [FirestoreProperty]
    public string userId { get; set; }
    [FirestoreProperty]
    public string employeeNumber { get; set; }
    [FirestoreProperty]
    public string leaderEmployeeId { get; set; }
    [FirestoreProperty]
    public string positonTitle { get; set; }
    [FirestoreProperty]
    public string emailAddress { get; set; }
    [FirestoreProperty]
    public string phoneNumber { get; set; }
    [FirestoreProperty]
    public string personalDecription { get; set; }
    [FirestoreProperty]
    public string managerId { get; set; }
    [FirestoreProperty]
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