using Google.Cloud.Firestore;
using Google.Type;

namespace InnovativeLife.DataAccess.Post;

[FirestoreData]
public class PostReactionsModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string postId { get; set; }
    [FirestoreProperty]
    public List<PostReactionModel> PostReactionss { get; set; } = new List<PostReactionModel>();
}