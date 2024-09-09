using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeSetAdminPrivilegeProcessor
{
    public  Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string uId, bool adminPrivilege);
}