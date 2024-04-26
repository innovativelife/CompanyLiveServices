using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Services.User.ServiceMessages;

namespace InnovativeLife.Services.User;

public interface IUserService
{
    public Task<UserCreateResponse> CreateUser(RequestContext userContext, UserCreateRequest request);

    public Task<WebResponse> SetAdminPrivilege(RequestContext userContext, string uId, bool AdminPrivilege);

    // public Task<WebResponse> ReadByUID(RequestContext userContext, string userUID);

    // public Task<WebResponse> ReadByIdentifier(RequestContext userContext, string identifier);

    // public Task<WebResponse> Save(RequestContext userContext, string userUID, UserModel userModel);
}
