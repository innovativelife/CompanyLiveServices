using Google.Cloud.Firestore;
using InnovativeLife.WebApi.Common;

namespace InnovativeLife.DataAccess.User;

[FirestoreData]
public class UserModel
{
    [FirestoreProperty]
    public string userUID { get; set; }
    [FirestoreProperty]
    public string identifier { get; set; }
    [FirestoreProperty]
    public string firstName { get; set; }
    [FirestoreProperty]
    public string lastName { get; set; }
    [FirestoreProperty]
    public string preferredName { get; set; }
    [FirestoreProperty]
    public bool active { get; set; }
    [FirestoreProperty]
    public List<TenantAccessModel> tenantAccessList { get; set; }

    public WebResponse Validate()
    {
        if (string.IsNullOrWhiteSpace(userUID))
            return StandardResponse.ErrorWithMessage("User UID mandatory (userUID)");

        if (string.IsNullOrWhiteSpace(identifier))
            return StandardResponse.ErrorWithMessage("User Identifier is mandatory (identifier)");

        if (string.IsNullOrWhiteSpace(firstName))
            return StandardResponse.ErrorWithMessage("First Name is mandatory (firstName)");

        if (string.IsNullOrWhiteSpace(lastName))
            return StandardResponse.ErrorWithMessage("Last Name mandatory (lastName)");

        if (string.IsNullOrWhiteSpace(preferredName))
            preferredName = firstName;

        if (active && (tenantAccessList == null || tenantAccessList.Count == 0))
            return StandardResponse.ErrorWithMessage("User must have access to at least one tenant or be inactive (tenantAccessList)");

        return StandardResponse.Success;
    }

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

    public WebResponse Validate()
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            return StandardResponse.ErrorWithMessage("Tenant Id is mandatory (tenantId)");

        return StandardResponse.Success;
    }
}