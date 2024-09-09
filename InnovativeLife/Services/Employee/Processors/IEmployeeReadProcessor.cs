using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeReadProcessor
{
    public Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID);
    public Task<EmployeeReadResponse> ReadByEmpoyeeNumber(IUserContext requestContext, string employeeNumber);
    public Task<EmployeeReadResponse> ReadByEmailAddress(IUserContext requestContext, string employeeNumber);

    // public Task<TenantReadSetResponse> ReadSet(IUserContext requestContext);
}