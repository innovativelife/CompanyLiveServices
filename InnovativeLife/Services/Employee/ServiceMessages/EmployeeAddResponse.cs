using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeAddResponse : ServiceResponseBase
{
         public EmployeeAddResponse(ResponseStatus status, string message) : base(status, message) { }
}