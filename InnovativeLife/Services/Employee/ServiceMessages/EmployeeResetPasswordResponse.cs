using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeResetPasswordResponse : ServiceResponseBase
{
     public EmployeeResetPasswordResponse(ResponseStatus status, string message) : base(status, message) { }
     public EmployeeResetPasswordResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public EmployeeResetPasswordResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
}