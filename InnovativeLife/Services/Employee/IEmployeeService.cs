using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Employee;

public interface IEmployeeService
{
    public Task<EmployeeAddResponse> Add(IUserContext requestContext, EmployeeAddRequest request);

    public Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string employeeUID, bool AdminPrivilege);

    public Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID);

    public Task<EmployeeReadResponse> ReadByEmpoyeeNumber(IUserContext requestContext, string employeeNumber);

    public Task<EmployeeReadResponse> ReadByEmailAddress(IUserContext requestContext, string emailAddress);

    public Task<EmployeeSaveResponse> Save(IUserContext requestContext, string employeeUID ,EmployeeSaveRequest request);
}
