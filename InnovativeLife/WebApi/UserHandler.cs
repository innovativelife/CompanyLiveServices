using Microsoft.Extensions.Logging;
using System.Text.Json;
using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Services.User;
using InnovativeLife.Services.User.ServiceMessages;

namespace InnovativeLife.WebApi;
public class UserHandler : IUserHandler
{
    ILogger<TenantHandler> _logger;
    IUserService _userService;

    public UserHandler(ILogger<TenantHandler> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public async Task<WebResponse> ExecuteService(RequestContext userContext, string method, string[] parameters, string body)
    {
        _logger.LogInformation($"Executing UserService for {method} and with {parameters.Length} parameters");

        if (parameters.Length == 0)
        {
            return StandardResponse.InvalidRequest;
        }

        if (parameters[0] == "Create" && method == "POST")
        {
            _logger.LogInformation("Creating user in the tenant");
            UserCreateRequest? userCreateRequest = JsonSerializer.Deserialize<UserCreateRequest>(body);
            var userCreateResponse = await _userService.CreateUser(userContext, userCreateRequest);
            return new WebResponse(userCreateResponse.Status, JsonSerializer.Serialize(userCreateResponse));
        }

        if (parameters[0] == "SetAdminPrivilege" && method == "POST")
        {
            if (parameters.Length < 3)
            {

            }

            _logger.LogInformation("Setting Admin Privilege for user");
            string uid = parameters[1];
            bool adminPrivilege = parameters[2].ToLower() == "true";

            return await _userService.SetAdminPrivilege(userContext, uid, adminPrivilege);
        }

        // if (parameters[0] == "ReadByUID")
        // {
        //     _logger.LogInformation("Performing Read by UID Action");
        //     return await _userService.ReadByUID(userContext, otherParameters[0]);
        // }
        // if (parameters[0] == "ReadByIdentifier")
        // {
        //     _logger.LogInformation("Performing Read by Identifier Action");
        //     return await _userService.ReadByIdentifier(userContext, otherParameters[0]);
        // }
        // else if (parameters[0] == "Save")
        // {
        //     _logger.LogInformation("Performing Save Action");
        //     UserModel? userModel = JsonSerializer.Deserialize<UserModel>(body);
        //     return await _userService.Save(userContext, otherParameters[0], userModel);
        // }

        return StandardResponse.InvalidRequest;
    }
}