using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.User.ServiceMessages;

public class UserCreateResponse : ServiceResponseBase
{
         public UserCreateResponse(ResponseStatus status, string message) : base(status, message) { }
}