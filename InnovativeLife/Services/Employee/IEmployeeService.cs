using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Employee;

public interface IEmployeeService
{
    public Task<EmployeeAddResponse> Add(IUserContext requestContext, EmployeeAddRequest request);

    public Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string employeeUID, bool AdminPrivilege);

    public Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID);

    public Task<EmployeeSaveResponse> Save(IUserContext requestContext, string employeeUID, EmployeeSaveRequest request);

    public Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber);
}
