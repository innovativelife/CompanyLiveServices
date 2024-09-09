using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeReadResponse : ServiceResponseBase
{
     public EmployeeReadResponse(ResponseStatus status, string message) : base(status, message) { }
     public EmployeeReadResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public EmployeeReadResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }

     public EmployeeItem? employee {set; get;}
}