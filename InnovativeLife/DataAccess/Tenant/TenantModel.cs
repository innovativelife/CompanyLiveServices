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
    public string primaryContactName {get; set;} = "";
    [FirestoreProperty]
    public string primaryContactEmail {get; set;} = "";
    [FirestoreProperty]
    public string primaryContactPhone {get; set;} = "";
    [FirestoreProperty]
    public string secondaryContactName {get; set;} = "";
    [FirestoreProperty]
    public string secondaryContactEmail {get; set;} = "";
    [FirestoreProperty]
    public string secondaryContactPhone {get; set;} = "";
    [FirestoreProperty]
    public DateTime renewalDate {get; set;} = DateTime.Today.AddYears(1);
    [FirestoreProperty]
    public bool active { get; set; }
}
