using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Employee;

public interface IEmployeeService
{
    public Task<EmployeeAddResponse> Add(IUserContext requestContext, string tenantId, EmployeeAddRequest request, bool allowRoot);

    public Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string tenantId, string employeeUID, bool AdminPrivilege, bool allowRoot);

    public Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string tenantId, string employeeUID, bool allowRoot);

    public Task<EmployeeSaveResponse> Save(IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest request);

    public Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber);

    public Task<EmployeeResetPasswordResponse> ResetPassword(IUserContext requestContext, string tenantId, string employeeUID, string newPassword);

}
