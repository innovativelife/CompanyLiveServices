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

    public async Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID)
    {
         _logger.LogInformation("Executing TenantService ReadByEmpoyeeNumber");

        if (string.IsNullOrWhiteSpace(employeeUID))
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage(MessageService.Employee_UID_Mandatory));
        }

        var result = await _employeeActions.ReadByEmployeeUID(employeeUID);

        if (result.Item1.Success)
        {
            var response = new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Employee Found");
            response.employee = getEmployeeItemFromEmployeeModel(result.Item2);
            return response;
        }
        else
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Employee not found.  Employee UID: {employeeUID}");
        }
    }

    public async Task<EmployeeReadResponse> ReadByEmpoyeeNumber(IUserContext requestContext, string employeeNumber)
    {
        _logger.LogInformation("Executing TenantService ReadByEmpoyeeNumber");

        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage(MessageService.Employee_Number_Mandatory));
        }

        var result = await _employeeActions.ReadByEmployeeNumber(employeeNumber);

        if (result.Item1.Success)
        {
            var response = new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Employee Found");
            response.employee = getEmployeeItemFromEmployeeModel(result.Item2);
            return response;
        }
        else
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Employee not found.  Employee Number: {employeeNumber}");
        }
    }

    public async Task<EmployeeReadResponse> ReadByEmailAddress(IUserContext requestContext, string emailAddress)
    {
         _logger.LogInformation("Executing TenantService ReadByEmailAddress");

        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, _messageService.GetMessage(MessageService.Email_Address_Mandatory));
        }

        var result = await _employeeActions.ReadByEmailAddress(emailAddress);

        if (result.Item1.Success)
        {
            var response = new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Employee Found");
            response.employee = getEmployeeItemFromEmployeeModel(result.Item2);
            return response;
        }
        else
        {
            return new EmployeeReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Employee not found.  Email address: {emailAddress}");
        }
    }

    private EmployeeItem getEmployeeItemFromEmployeeModel(DataAccess.Employee.Employee employee)
    {
        return new EmployeeItem(
            employee.tenantId,
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