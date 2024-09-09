using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee.ServiceMessages;

public class EmployeeSaveResponse : ServiceResponseBase
{
    public EmployeeSaveResponse(ResponseStatus status, string message) : base(status, message) { }
    public EmployeeSaveResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }

    public EmployeeItem? employee { get; set; }
}