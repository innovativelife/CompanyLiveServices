using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Services.Employee.Processors;

namespace InnovativeLife.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private ILogger<EmployeeService> _logger;
    private IEmployeeAddProcessor _employeeCreateProcessor;
    private IIdentityService _identityService;

    public EmployeeService(ILogger<EmployeeService> logger, IIdentityService identityService, IEmployeeAddProcessor employeeCreateProcessor)
    {
        _logger = logger;
        _identityService = identityService;
        _employeeCreateProcessor = employeeCreateProcessor;
    }

    public async Task<EmployeeAddResponse> AddEmployee(IUserContext requestContext, EmployeeAddRequest request)
    {
        return await _employeeCreateProcessor.AddEmployee(requestContext, request);
    }

    public async Task<WebResponse> SetAdminPrivilege(IUserContext requestContext, string uId, bool AdminPrivilege)
    {
        _logger.LogInformation("EmployeeService.SetAdminPrivilege: Executing SetAdminPrivilege Service");

        if (requestContext.developmentMode)
        {
            return new WebResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "EmployeeService.SetAdminPrivilege: Development mode - Setting Admin Privilege skipped");
        }

        try
        {
            var result = await _identityService.SetAdminAuthorisationForUser(requestContext.tenantId, uId, AdminPrivilege, requestContext);
            if (!result.Item1)
            {
                _logger.LogError($"EmployeeService.SetAdminPrivilege: Failed to set admin status for User for {uId}");
                return new WebResponse(WebResponse.StatusTypes.Error, "Failed to set admin status for User for {uId}");
            }

            return new WebResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"User privilege for {uId} set to: {AdminPrivilege}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeService.SetAdminPrivilege: Exception caught in SetAdminPrivilege service: {ex.Message}");
            return new WebResponse(WebResponse.StatusTypes.Error, ex.Message);
        }
    }

    // async Task<WebResponse> IUserService.ReadByIdentifier(RequestContext requestContext, string identifier)
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

    // async Task<WebResponse> IUserService.ReadByUID(RequestContext requestContext, string userUID)
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

    // async Task<WebResponse> IUserService.Save(RequestContext requestContext, string userUID, UserModel userModel)
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