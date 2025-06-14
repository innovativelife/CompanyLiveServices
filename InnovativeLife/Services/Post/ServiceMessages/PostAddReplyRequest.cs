using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostAddReplyResquest : RequestBase
{

    // [Required(ErrorMessage = "timeSent must be provided.")]
    // public string timeSent { get; set; } = "";

    [Required(ErrorMessage = "employeeUID must be provided.")]
    public string employeeUID { get; set; } = "";

    [Required(ErrorMessage = "message must be provided.")]
    public string message { get; set; } = "";

    // [Required(ErrorMessage = "tenantId must be provided.")]
    // public string tenantId { get; set; } = "";

    // [Required(ErrorMessage = "postId must be provided.")]
    // public string postId { get; set; } = "";

}