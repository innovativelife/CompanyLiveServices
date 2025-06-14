using Google.Cloud.Firestore;
using Google.Type;

namespace InnovativeLife.DataAccess.Post;

[FirestoreData]
public class PostReplyModel
{
    [FirestoreProperty]
    public string postReplyId { get; set; }
    [FirestoreProperty]
    public string message { get; set; }
    [FirestoreProperty]
    public string employeeUID { get; set; }
    [FirestoreProperty]
    public string timeSent { get; set; }
}