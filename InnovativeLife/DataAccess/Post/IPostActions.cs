using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Post;

public interface IPostActions
{
    public Task<Tuple<DalResponse, PostModel?>> ReadByPostId(string tenantId, string postId);
    public Task<Tuple<DalResponse, List<PostModel>>> Search(string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message);

    public Task<DalResponse> Save(string tenantId, PostModel postModel);
}