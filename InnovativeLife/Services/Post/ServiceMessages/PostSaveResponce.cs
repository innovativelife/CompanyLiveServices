using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostSaveResponse : ServiceResponseBase
{
    public PostSaveResponse(ResponseStatus status, string message) : base(status, message) { }
    public PostSaveResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
    public PostSaveResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }

    public PostItem postItem { get; set; }
}