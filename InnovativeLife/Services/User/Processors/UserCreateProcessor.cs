using Microsoft.Extensions.Logging;
using InnovativeLife.Services.User.ServiceMessages;
using InnovativeLife.Common;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.DataAccess.User;

namespace InnovativeLife.Services.User.Processors;

public class UserCreateProcessor : IUserCreateProcessor
{
    private readonly ILogger<IUserCreateProcessor> _logger;
    private readonly IIdentityService _identityService;
    private readonly IUserActions _userActions;

    public UserCreateProcessor(ILogger<IUserCreateProcessor> logger, IIdentityService identityService, IUserActions userActions)
    {
        _logger = logger;
        _identityService = identityService;
        _userActions = userActions;
    }

    public async Task<UserCreateResponse> CreateUser(RequestContext userContext, UserCreateRequest request)
    {
        _logger.LogInformation("Executing CreateUser Service");

        if (userContext.developmentMode)
        {
            return new UserCreateResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Development mode - User creation skipped");
        }

        try
        {
            // Add user to the tenancy of the executing user
            var addUserToTenantResult = await _identityService.AddUserToTenant(userContext.tenantId, request.displayName, request.email, request.phoneNumber, request.initialPassword);
            if (!addUserToTenantResult.Success)
            {
                return new UserCreateResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Failed to create user: {addUserToTenantResult.message}");
            }

            // Create the user in the DB
            var userModel = new UserModel();
            userModel.active = true;
            userModel.firstName = request.firstName;
            userModel.lastName = request.lastName;
            userModel.preferredName = request.preferredName;
            userModel.userUID = addUserToTenantResult.uId;
            var saveUserResult = await _userActions.Save(addUserToTenantResult.uId, userModel);

            if (saveUserResult.Item1.Success)
            {
                return new UserCreateResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "User created");
            }
            else
            {
                return new UserCreateResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "User could not be added due to unexpected DB error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught in CreateUser service: {ex.Message}");
            return new UserCreateResponse(Common.ServiceResponseBase.ResponseStatus.Exception, ex.Message);
        }
    }
}