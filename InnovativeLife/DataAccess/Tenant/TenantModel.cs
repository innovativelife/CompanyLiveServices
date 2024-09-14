using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.Tenant;

[FirestoreData]
public class TenantModel
{
    [FirestoreProperty]
    public string tenantId { get; set; } = "";
    [FirestoreProperty]
    public string identityManagerTenantId { get; set; } = "";
    [FirestoreProperty]
    public string tenantName { get; set; } = "";
    [FirestoreProperty]
    public string customerName {get; set; } = "";
    [FirestoreProperty]
    public string primaryAdministratorEmployeeUID  {get; set; } = "";
    [FirestoreProperty]
    public string secondaryAdministratorEmployeeUID  {get; set; } = "";    
    [FirestoreProperty]
    public DateTime renewalDate {get; set;} = DateTime.Today.AddYears(1);
    [FirestoreProperty]
    public bool active { get; set; }
}
