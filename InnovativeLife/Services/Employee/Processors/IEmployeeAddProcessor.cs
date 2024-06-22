using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Common;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeAddProcessor
{
    public  Task<EmployeeAddResponse> AddEmployee(IRequestContext requestContext, EmployeeAddRequest request);
}