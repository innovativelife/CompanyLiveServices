using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeSetAdminPrivilegeProcessor : IEmployeeSetAdminPrivilegeProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IIdentityService _identityService;
    private readonly IEmployeeActions _employeeActions;

    public EmployeeSetAdminPrivilegeProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IIdentityService identityService, IEmployeeActions employeeActions)
    {
        _logger = logger;
        _messageService = messageService;
        _identityService = identityService;
        _employeeActions = employeeActions;
    }

    public async Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string employeeUID, bool adminPrivilege)
    {
        _logger.LogInformation("EmployeeService.SetAdminPrivilege: Executing SetAdminPrivilege Service");
        try
        {
            if (requestContext.developmentMode)
            {
                    _logger.LogError("In Development mode - Skipping setting Admin Claims");
            }
            else
            {
                var setClaimsResult = await _identityService.SetAdminAuthorisationForUser(requestContext.tenantId, requestContext.uId, adminPrivilege, requestContext);

                if (!setClaimsResult.Item1)
                {
                    _logger.LogError($"Error using identity service to set admin priviledge for {employeeUID}");
                    return new EmployeeSetAdminPrivilegeResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Failed to set admin status for employeeUID for {employeeUID}");
                }
            }

            var result = await _employeeActions.SetAdminPrivilege(employeeUID, adminPrivilege);
            if (!result.Success)
            {
                _logger.LogError($"EmployeeService.SetAdminPrivilege: Failed to set admin status for User for {employeeUID}");
                return new EmployeeSetAdminPrivilegeResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Failed to set admin status for employeeUID for {employeeUID}");
            }

            return new EmployeeSetAdminPrivilegeResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"User privilege for employee {employeeUID} set to: {adminPrivilege}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeService.SetAdminPrivilege: Exception caught in SetAdminPrivilege service: {ex.Message}");
            return new EmployeeSetAdminPrivilegeResponse(Common.ServiceResponseBase.ResponseStatus.Exception, ex.Message);
        }
    }
}
