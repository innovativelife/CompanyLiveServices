using InnovativeLife.Services.Common;
using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeAddFavoriteResponse : ServiceResponseBase
{
     public EmployeeAddFavoriteResponse(ResponseStatus status, string message) : base(status, message) { }
     public EmployeeAddFavoriteResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public EmployeeAddFavoriteResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
}