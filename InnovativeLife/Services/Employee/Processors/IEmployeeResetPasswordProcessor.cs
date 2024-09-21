using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeResetPasswordProcessor
{
    public  Task<EmployeeResetPasswordResponse> ResetPassword(IUserContext requestContext, string tenantId, string employeeUID, string newPassword);
}