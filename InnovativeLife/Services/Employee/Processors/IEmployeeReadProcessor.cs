using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeReadProcessor
{
    public Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID);
    public Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber);
}