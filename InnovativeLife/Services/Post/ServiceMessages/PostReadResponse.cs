using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostReadResponse : ServiceResponseBase
{
    public PostReadResponse(ResponseStatus status, string message) : base(status, message) { }

    public PostItem post { get; set; }
}