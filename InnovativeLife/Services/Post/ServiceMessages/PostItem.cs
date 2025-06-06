using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostItem
{
    public PostItem(string tenantId,
       string postId,
       string timeSent)
    {
        this.tenantId = tenantId;
        this.postId = postId;
        this.timeSent = timeSent;
    }

    public PostItem(
        string tenantId,
        string postId,
        string timeSent,
        string status,
        string sendTo,
        string employeeUID,
        string message,
        string imageURL
    )
    {
        this.tenantId = tenantId;
        this.postId = postId;
        this.timeSent = timeSent;
        this.status = status;
        this.sendTo = sendTo;
        this.employeeUID = employeeUID;
        this.message = message;
        this.imageURL = imageURL;
    }
    public string tenantId { get; set; }
    public string postId { get; set; }
    public string timeSent { get; set; }
    public string status { get; set; }
    public string sendTo { get; set; }
    public string employeeUID { get; set; }
    public string message { get; set; }
    public string imageURL { get; set; }
}

