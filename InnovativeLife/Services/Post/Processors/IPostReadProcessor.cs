using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Post.Processors;

public interface IPostReadProcessor
{
    public Task<PostReadResponse> ReadSingleton(IUserContext requestContext, string tenantId, string postId);
    public Task<PostSearchResponse> SearchPost(IUserContext requestContext, string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message);

    public Task<PostRepliesReadResponse> ReadReplies(IUserContext requestContext, string tenantId, string postId);
}