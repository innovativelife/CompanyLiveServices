using Google.Cloud.Firestore;
using Google.Type;

namespace InnovativeLife.DataAccess.Post;

[FirestoreData]
public class PostRepliesModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string postId { get; set; }
    [FirestoreProperty]
    public List<PostReplyModel> PostReplies { get; set; } = new List<PostReplyModel>();
}