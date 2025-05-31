using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Post.Processors;

public interface IPostSaveProcessor
{
    public Task<PostSaveResponse> Save(IUserContext requestContext, string tenantId, PostSaveRequest saveRequest);
}