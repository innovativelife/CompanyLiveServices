using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeSaveProcessor : IEmployeeSaveProcessor
{
    private readonly ILogger<IEmployeeSaveProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IEmployeeActions _employeeActions;

    public EmployeeSaveProcessor(ILogger<IEmployeeSaveProcessor> logger, IMessageService messageService, IEmployeeActions employeeActions)
    {
        _logger = logger;
        _messageService = messageService;
        _employeeActions = employeeActions;
    }

    public async Task<EmployeeSaveResponse> SaveEmployee(IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest request)
    {
        _logger.LogInformation($"EmployeeSaveProcessor.SaveEmployee: Executing Save Service for Employee UID {employeeUID}");

        try
        {
            // Basic validation of request data
            var validationResult = request.Validate();
            if (validationResult.Count > 0)
            {
                return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
            }

            // Retrieve existing details
            var existingEmployee = await _employeeActions.ReadByEmployeeUID(tenantId, employeeUID);
            if (!existingEmployee.Item1.Success)
            {
                return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Employee with Employee UID {employeeUID} does not exist");
            }

            // Check employee is in correct tenant
            if (!existingEmployee.Item2.tenantId.Equals(tenantId))
            {
                _logger.LogCritical("EmployeeSaveProcessor.SaveEmployee: Security violation - Attempt made to update employee in another tenant");
                return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.BadRequest, "Invalid operation");
            }

            // Check uniqueness of key employeeNumber (if it is being updated)
            if (existingEmployee.Item2.employeeNumber != request.employeeNumber)
            {
                var readByEmployeeNumberResult = await _employeeActions.ReadByEmployeeNumber(tenantId, request.employeeNumber);
                if (readByEmployeeNumberResult.Item1.Success)
                {
                    return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "Employee already exists with this Employee Number - must be unique");
                }
            }

            // Check uniqueness of key email (if it is being updated)
            if (existingEmployee.Item2.email != request.email)
            {
                var readByEmailResult = await _employeeActions.ReadByEmail(tenantId, request.email);
                if (readByEmailResult.Item1.Success)
                {
                    return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "Employee already exists with this Email Address - must be unique");
                }
            }

            // Check leader exists (if it is being updated)
            if (existingEmployee.Item2.leaderEmployeeNumber != request.leaderEmployeeNumber)
            {
                if (request.leaderEmployeeNumber.Trim().Length > 0)
                {
                    var readByLeaderEmployeeNumberResult = await _employeeActions.ReadByEmployeeNumber(tenantId, request.leaderEmployeeNumber);
                    if (!readByLeaderEmployeeNumberResult.Item1.Success)
                    {
                        return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "No employee exists with this Leader Employee Number");
                    }
                }
            }

            // Create the employee for user in the DB
            var employeeModel = new DataAccess.Employee.Employee
            {
                tenantId = tenantId,
                firstName = request.firstName,
                lastName = request.lastName,
                preferredName = request.preferredName,
                employeeUID = employeeUID,
                email = request.email,
                phoneNumber = request.phoneNumber,
                employeeNumber = request.employeeNumber,
                leaderEmployeeNumber = request.leaderEmployeeNumber,
                positionTitle = request.positionTitle,
                personalDescription = request.personalDescription,
                avatarURL = request.avatarURL,
                active = request.active,
                adminPrivilege = existingEmployee.Item2.adminPrivilege
            };

            var saveUserResult = await _employeeActions.Save(tenantId, employeeUID, employeeModel);

            if (saveUserResult.Success)
            {

                var response = new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Employee updated successfully");
                response.employee = new EmployeeItem
                (
                    tenantId,
                    employeeModel.employeeUID,
                    employeeModel.email,
                    employeeModel.phoneNumber,
                    employeeModel.firstName,
                    employeeModel.lastName,
                    employeeModel.preferredName,
                    employeeModel.employeeNumber,
                    employeeModel.leaderEmployeeNumber,
                    employeeModel.positionTitle,
                    employeeModel.personalDescription,
                    employeeModel.avatarURL,
                    employeeModel.active,
                    employeeModel.adminPrivilege
                );

                return response;
            }
            else
            {
                return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Employee could not be added due to unexpected error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeAddProcessor.AddEmployee: Exception caught in SaveEmployee service: {ex.Message}");
            return new EmployeeSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Unexpected error occurred while saving employee");
        }
    }
}