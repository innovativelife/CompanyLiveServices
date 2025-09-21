using System.ComponentModel.DataAnnotations;
using InnovativeLife.Services.Common;
using InnovativeLife.DataAccess.Post;

namespace InnovativeLife.Services.Post.ServiceMessages;

public class PostAddReactionResquest : RequestBase
{
    [Required(ErrorMessage = "employeeUID must be provided.")]
    public string employeeUID { get; set; } = "";

    [Required(ErrorMessage = "reaction must be provided.")]
    public ReactionType reaction { get; set; }

}