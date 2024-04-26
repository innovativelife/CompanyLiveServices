using InnovativeLife.Services.User.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.User.Processors;

public interface IUserCreateProcessor
{
    public  Task<UserCreateResponse> CreateUser(RequestContext userContext, UserCreateRequest request);
}