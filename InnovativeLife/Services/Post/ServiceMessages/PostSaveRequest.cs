using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostSaveRequest : RequestBase
{

    [Required(ErrorMessage = "status must be provided.")]
    public string status { get; set; } = "";

    [Required(ErrorMessage = "sendTo must be provided.")]
    public string sendTo { get; set; } = "";

    [Required(ErrorMessage = "message must be provided.")]
    public string message { get; set; } = "";

    [Required(ErrorMessage = "employeeUID must be provided.")]
    public string employeeUID { get; set; } = "";

}