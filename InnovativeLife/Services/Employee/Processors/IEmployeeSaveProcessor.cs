using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeSaveProcessor
{
    public  Task<EmployeeSaveResponse> SaveEmployee(IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest request);
}