using InnovativeLife.Services.Common;
using InnovativeLife.DataAccess.Post;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostReaction
{
    public PostReaction(string tenantId,
       string postId)
    {
        this.tenantId = tenantId;
        this.postId = postId;
    }

    public PostReaction(
        string tenantId,
        string postId,
        string timeSent,
        string employeeUID,
        ReactionType reaction
    )
    {
        this.tenantId = tenantId;
        this.postId = postId;
        this.timeSent = timeSent;
        this.employeeUID = employeeUID;
        this.reaction = reaction;
    }
    public string tenantId { get; set; }
    public string postId { get; set; }
    public string timeSent { get; set; }
    public string employeeUID { get; set; }
    public ReactionType reaction { get; set; }
}

