using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostReply
{
    public PostReply(string tenantId,
       string postId)
    {
        this.tenantId = tenantId;
        this.postId = postId;
    }

    public PostReply(
        string tenantId,
        string postId,
        string timeSent,
        string employeeUID,
        string message
    )
    {
        this.tenantId = tenantId;
        this.postId = postId;
        this.timeSent = timeSent;
        this.employeeUID = employeeUID;
        this.message = message;
    }
    public string tenantId { get; set; }
    public string postId { get; set; }
    public string timeSent { get; set; }
    public string employeeUID { get; set; }
    public string message { get; set; }
}

