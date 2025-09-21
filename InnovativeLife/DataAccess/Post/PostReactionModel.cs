using Google.Cloud.Firestore;
using Google.Type;
using System.Text.Json.Serialization;

namespace InnovativeLife.DataAccess.Post;

[FirestoreData]
public class PostReactionModel
{
    [FirestoreProperty]
    public string postReactionId { get; set; }
    [FirestoreProperty]
    public ReactionType reaction { get; set; }
    [FirestoreProperty]
    public string employeeUID { get; set; }
    [FirestoreProperty]
    public string timeSent { get; set; }
}