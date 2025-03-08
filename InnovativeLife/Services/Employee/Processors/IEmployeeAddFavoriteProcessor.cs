using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.Employee.Processors;

public interface IEmployeeAddFavoriteProcessor
{
    public  Task<EmployeeAddFavoriteResponse> EmployeeAddFavoriteEmployee(IUserContext requestContext, string tenantId, string employeeUID, string favoriteEmployeeId);
}