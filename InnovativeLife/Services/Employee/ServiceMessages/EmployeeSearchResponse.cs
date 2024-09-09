using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeSearchResponse : ServiceResponseBase
{
     public EmployeeSearchResponse(ResponseStatus status, string message) : base(status, message) { }
     public EmployeeSearchResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public EmployeeSearchResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
     public List<EmployeeItem> employees {set; get;} = new List<EmployeeItem>();
}