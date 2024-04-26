using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.User;
using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.User.ServiceMessages;
using InnovativeLife.Services.User.Processors;

namespace InnovativeLife.Services.User;

public class UserService : IUserService
{
    private ILogger<UserService> _logger;
    private IUserCreateProcessor _userCreateProcessor;
    private IIdentityService _identityService;

    public UserService(ILogger<UserService> logger, IIdentityService identityService, IUserCreateProcessor userCreateProcessor)
    {
        _logger = logger;
        _identityService = identityService;
        _userCreateProcessor = userCreateProcessor;
    }

    public async Task<UserCreateResponse> CreateUser(RequestContext userContext, UserCreateRequest request)
    {
        return await _userCreateProcessor.CreateUser(userContext, request);
    }

    public async Task<WebResponse> SetAdminPrivilege(RequestContext userContext, string uId, bool AdminPrivilege)
    {
        _logger.LogInformation("Executing SetAdminPrivilege Service");

        if (userContext.developmentMode)
        {
            return new WebResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Development mode - Setting Admin Privilege skipped");
        }

        try
        {
            var result = await _identityService.SetAdminAuthorisationForUser(userContext.tenantId, uId, AdminPrivilege);
            if (!result.Item1)
            {
                return new WebResponse(WebResponse.StatusTypes.Error, "Failed to set admin status");
            }

            return new WebResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"User privilegef for {uId} set to: {AdminPrivilege}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught in SetAdminPrivilege service: {ex.Message}");
            return new WebResponse(WebResponse.StatusTypes.Error, ex.Message);
        }
    }

    // async Task<WebResponse> IUserService.ReadByIdentifier(RequestContext userContext, string identifier)
    // {
    //     _logger.LogInformation("Executing User Read by Identifier");

    //     if (string.IsNullOrWhiteSpace(identifier))
    //     {
    //         return StandardResponse.InvalidRequest;
    //     }

    //     var result = await _userActions.ReadByIdentifier(identifier);

    //     if (result.Item1.Success)
    //     {
    //         return StandardResponse.SuccessWithBody(JsonSerializer.Serialize(result.Item2));
    //     }
    //     else
    //     {
    //         return result.Item1;
    //     }
    // }

    // async Task<WebResponse> IUserService.ReadByUID(RequestContext userContext, string userUID)
    // {
    //     _logger.LogInformation("Executing User Read by Identifier");

    //     if (string.IsNullOrWhiteSpace(userUID))
    //     {
    //         return StandardResponse.InvalidRequest;
    //     }

    //     var result = await _userActions.ReadByUID(userUID);

    //     if (result.Item1.Success)
    //     {
    //         return StandardResponse.SuccessWithBody(JsonSerializer.Serialize(result.Item2));
    //     }
    //     else
    //     {
    //         return result.Item1;
    //     }
    // }

    // async Task<WebResponse> IUserService.Save(RequestContext userContext, string userUID, UserModel userModel)
    // {
    //     _logger.LogInformation("Executing User Save");

    //     if (string.IsNullOrWhiteSpace(userUID))
    //     {
    //         return StandardResponse.InvalidRequest;
    //     }

    //     userModel.userUID = userUID;
    //     var result = await _userActions.Save(userUID, userModel);

    //     _logger.LogInformation("UiShellConfig Saved");
    //     return result;
    // }
}