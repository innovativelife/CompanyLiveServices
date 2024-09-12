using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeAddProcessor
{
    public  Task<EmployeeAddResponse> AddEmployee(IUserContext requestContext, string tenantId, EmployeeAddRequest request);
}