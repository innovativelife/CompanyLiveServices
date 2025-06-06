using Google.Cloud.Firestore;
using InnovativeLife.WebApi;

namespace InnovativeLife.DataAccess.Employee;

[FirestoreData]
public class Employee
{
  [FirestoreProperty]
  public string tenantId { get; set; } = "";
  [FirestoreProperty]
  public string employeeUID { get; set; } = "";
  [FirestoreProperty]
  public bool adminPrivilege { get; set; }
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
  public string positionTitle { get; set; } = "";
  [FirestoreProperty]
  public string personalDescription { get; set; } = "";
  [FirestoreProperty]
  public string avatarURL { get; set; } = "";
  [FirestoreProperty]
  public bool active { get; set; }
  // public List<EmployeeGroupMembership> employeeGroupMembershipList { get; set; }
  // public List<EmployeeGroupMembership> favoritedEmployeeList { get; set; }
}

// [FirestoreData]
// public class EmployeeGroupMembership
// {
//     [FirestoreProperty]
//     public string employeeNumber { get; set; }
//     [FirestoreProperty]
//     public string employeeGroupMembershipType { get; set; }
// }

[FirestoreData]
public class FavoritedEmployeeModel
{
  [FirestoreProperty]
  public string employeeNumber { get; set; }
}

// [FirestoreData]
// public class TenantAccessModel
// {
//     [FirestoreProperty]
//     public string tenantId { get; set; }
//     [FirestoreProperty]
//     public string employeeNumber { get; set; }
//     [FirestoreProperty]
//     public string employeeGroupMembershipType { get; set; }
//     [FirestoreProperty]
//     public bool active { get; set; }
// }