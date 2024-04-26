using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.Tenant;

[FirestoreData]
public class TenantModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string identityManagerTenantId { get; set; }
    [FirestoreProperty]
    public string tenantName { get; set; }
    [FirestoreProperty]
    public bool active { get; set; }
}
