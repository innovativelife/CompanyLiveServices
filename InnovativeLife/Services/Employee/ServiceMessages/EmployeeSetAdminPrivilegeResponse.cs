using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeSetAdminPrivilegeResponse : ServiceResponseBase
{
     public EmployeeSetAdminPrivilegeResponse(ResponseStatus status, string message) : base(status, message) { }
     public EmployeeSetAdminPrivilegeResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public EmployeeSetAdminPrivilegeResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
}