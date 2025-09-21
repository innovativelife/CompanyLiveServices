using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostReactionReadResponse : ServiceResponseBase
{
    public PostReactionReadResponse(ResponseStatus status, string reaction) : base(status, reaction) { }
    public List<PostReaction> reactions { get; set; } = new List<PostReaction>();

}