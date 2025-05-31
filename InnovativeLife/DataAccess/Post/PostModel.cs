using Google.Cloud.Firestore;
using Google.Type;

namespace InnovativeLife.DataAccess.Post;

[FirestoreData]
public class PostModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string postId { get; set; }
    [FirestoreProperty]
    public string timeSent { get; set; }
    [FirestoreProperty]
    public string status { get; set; }
    [FirestoreProperty]
    public string sendTo { get; set; }
    [FirestoreProperty]
    public string employeeUID { get; set; }
    [FirestoreProperty]
    public string message { get; set; }
}