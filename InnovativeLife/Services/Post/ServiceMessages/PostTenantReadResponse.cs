using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;


public class PostSearchResponse : ServiceResponseBase
{
    public PostSearchResponse(ResponseStatus status, string message) : base(status, message) { }
    public PostSearchResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
    public PostSearchResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
    public List<PostItem> posts { set; get; } = new List<PostItem>();
}