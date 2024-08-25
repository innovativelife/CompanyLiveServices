using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Employee;

public interface IEmployeeService
{
    public Task<EmployeeAddResponse> AddEmployee(IUserContext requestContext, EmployeeAddRequest request);

    public Task<WebResponse> SetAdminPrivilege(IUserContext requestContext, string uId, bool AdminPrivilege);

    // public Task<WebResponse> ReadByUID(RequestContext requestContext, string userUID);

    // public Task<WebResponse> ReadByIdentifier(RequestContext requestContext, string identifier);

    // public Task<WebResponse> Save(RequestContext requestContext, string userUID, UserModel userModel);
}
