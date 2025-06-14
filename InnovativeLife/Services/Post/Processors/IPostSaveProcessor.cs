using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.DataAccess.Post;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Post.Processors;

public interface IPostSaveProcessor
{
    public Task<PostSaveResponse> Save(IUserContext requestContext, string tenantId, PostSaveRequest saveRequest);

    public Task<PostAddReplyResponse> AddPostReply(string tenantId, string postId, PostAddReplyResquest postReply);
}