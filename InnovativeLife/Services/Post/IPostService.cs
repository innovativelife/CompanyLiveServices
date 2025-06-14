using InnovativeLife.Security;
using InnovativeLife.Services.Post.ServiceMessages;

namespace InnovativeLife.Services.Post;

public interface IPostService
{
    public Task<PostReadResponse> Read(IUserContext requestContext, string tenantId, string postId);

    public Task<PostRepliesReadResponse> ReadReplies(IUserContext requestContext, string tenantId, string postId);

    public Task<PostSearchResponse> SearchPost(IUserContext requestContext, string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message);

    public Task<PostSaveResponse> Save(IUserContext requestContext, string tenantId, PostSaveRequest postModel);

    public Task<PostAddReplyResponse> AddPostReply(IUserContext requestContext, string tenantId, string postId, PostAddReplyResquest postReply);
}
