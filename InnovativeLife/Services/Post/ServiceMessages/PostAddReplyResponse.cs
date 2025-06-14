using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostAddReplyResponse : ServiceResponseBase
{
    public PostAddReplyResponse(ResponseStatus status, string message) : base(status, message) { }
    public PostAddReplyResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
    public PostAddReplyResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }

    public PostReply postReply { get; set; }
}