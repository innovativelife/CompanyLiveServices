using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Localization;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeResetPasswordProcessor : IEmployeeResetPasswordProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IIdentityService _identityService;

    public EmployeeResetPasswordProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IIdentityService identityService)
    {
        _logger = logger;
        _messageService = messageService;
        _identityService = identityService;
    }

    public async Task<EmployeeResetPasswordResponse> ResetPassword(IUserContext requestContext, string tenantId, string employeeUID, string newPassword)
    {
        _logger.LogInformation("EmployeeResetPasswordProcessor.ResetPassword: Executing ResetPassword Service");
        try
        {
            
            var result = await _identityService.ResetUserPassword(tenantId, employeeUID, newPassword, requestContext);
            if (!result.Success)
            {
                return new EmployeeResetPasswordResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Error resetting password- {result.Message}");
            }

            return new EmployeeResetPasswordResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"Password reset for employee {employeeUID}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeService.SetAdminPrivilege: Exception caught in SetAdminPrivilege service: {ex.Message}");
            return new EmployeeResetPasswordResponse(Common.ServiceResponseBase.ResponseStatus.Exception, ex.Message);
        }
    }
}
