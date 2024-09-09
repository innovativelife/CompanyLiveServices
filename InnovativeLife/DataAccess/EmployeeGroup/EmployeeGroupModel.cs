using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.EmployeeGroup;

[FirestoreData]
public class EmployeeGroupModel
{
    [FirestoreProperty]
    public string employeeGroupId { get; set; }
    [FirestoreProperty]
    public string tennantId { get; set; }
    [FirestoreProperty]
    public string groupName { get; set; }
    [FirestoreProperty]
    public string groupType { get; set; }
    [FirestoreProperty]
    public List<GroupAssociation> groupAssociationList { get; set; }
    [FirestoreProperty]
    public List<GroupMember> groupMemberList { get; set; }
}

[FirestoreData]
public class GroupAssociation
{
    [FirestoreProperty]
    public string employeeGroupId { get; set; }
    [FirestoreProperty]
    public string groupAssociationType { get; set; }
}

[FirestoreData]
public class GroupMember
{
    [FirestoreProperty]
    public string employeeNumber { get; set; }
    [FirestoreProperty]
    public string employeeGroupMembershipType { get; set; }
}