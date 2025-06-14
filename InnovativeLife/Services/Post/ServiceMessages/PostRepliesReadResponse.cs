using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostRepliesReadResponse : ServiceResponseBase
{
    public PostRepliesReadResponse(ResponseStatus status, string message) : base(status, message) { }
    public List<PostReply> replies { get; set; } = new List<PostReply>();

}