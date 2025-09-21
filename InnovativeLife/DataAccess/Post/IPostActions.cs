using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Post;

public interface IPostActions
{
    public Task<Tuple<DalResponse, PostModel?>> ReadByPostId(string tenantId, string postId);
    public Task<Tuple<DalResponse, List<PostModel>>> Search(string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message);

    public Task<DalResponse> Save(string tenantId, PostModel postModel);

    public Task<DalResponse> AddPostReply(string tenantId, string PostReplyId, PostReplyModel postReply);
    public Task<Tuple<DalResponse, PostRepliesModel?>> ReadPostReplies(string tenantId, string PostId);

    public Task<DalResponse> AddPostReaction(string tenantId, string PostReactionId, PostReactionModel postReaction);
    public Task<Tuple<DalResponse, PostReactionsModel?>> ReadPostReactions(string tenantId, string PostId);
}