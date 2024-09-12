using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;


namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeReadProcessor : IEmployeeReadProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IEmployeeActions _employeeActions;
    public EmployeeReadProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IEmployeeActions employeeActions)
    {
        _logger = logger;
        _messageService = messageService;
        _employeeActions = employeeActions;
    }

    public async Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string tenantId, string employeeUID)
    {
        _logger.LogInformation("Executing TenantService ReadByEmpoyeeNumber");

        if (string.IsNullOrWhiteSpace(employeeUID))
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage(MessageService.Employee_UID_Mandatory));
        }

        var result = await _employeeActions.ReadByEmployeeUID(tenantId, employeeUID);

        if (result.Item1.Success)
        {
            var response = new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Employee Found");
            response.employee = getEmployeeItemFromEmployeeModel(tenantId, result.Item2);
            return response;
        }
        else
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Employee not found.  Employee UID: {employeeUID}");
        }
    }

    public async Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber)
    {
        var searchResult = await _employeeActions.Search(requestContext.tenantId, employeeNumber, email, firstName, lastName, leaderEmployeeNumber);

        if (searchResult.Item1.Success)
        {
            var employeeList = new List<EmployeeItem>();
            foreach (var item in searchResult.Item2)
            {
                employeeList.Add(getEmployeeItemFromEmployeeModel(tenantId, item));
            }
            var response = new EmployeeSearchResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"{employeeList.Count} Employee(s) Found");
            response.employees = employeeList;
            return response;
        }

        return new EmployeeSearchResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "Employee search returned no results");
    }

    private EmployeeItem getEmployeeItemFromEmployeeModel(string tenantId, DataAccess.Employee.Employee employee)
    {
        return new EmployeeItem(
            tenantId,
            employee.employeeUID,
            employee.email,
            employee.phoneNumber,
            employee.firstName,
            employee.lastName,
            employee.preferredName,
            employee.employeeNumber,
            employee.leaderEmployeeNumber,
            employee.positionTitle,
            employee.personalDescription,
            employee.active,
            employee.adminPrivilege
        );
    }
}