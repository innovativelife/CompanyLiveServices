using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostAddReactionResponse : ServiceResponseBase
{
    public PostAddReactionResponse(ResponseStatus status, string reaction) : base(status, reaction) { }
    public PostAddReactionResponse(ResponseStatus status, List<string> reactions) : base(status, reactions) { }
    public PostAddReactionResponse(DalResponse.ResponseStatus status, string reaction) : base(status, reaction) { }

    public PostReaction postReaction { get; set; }
}