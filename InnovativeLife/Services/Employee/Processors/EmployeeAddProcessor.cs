using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;
using InnovativeLife.Services.Tenant;
using InnovativeLife.DataAccess.Tenant;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeAddProcessor : IEmployeeAddProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IIdentityService _identityService;
    private readonly IEmployeeActions _employeeActions;
    private readonly ITenantActions _tenantActions;

    public EmployeeAddProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IIdentityService identityService, IEmployeeActions employeeActions, ITenantActions tenantActions)
    {
        _logger = logger;
        _messageService = messageService;
        _identityService = identityService;
        _employeeActions = employeeActions;
        _tenantActions = tenantActions;
    }

    public async Task<EmployeeAddResponse> AddEmployee(IUserContext requestContext, string tenantId, EmployeeAddRequest request)
    {
        _logger.LogInformation($"EmployeeAddProcessor.AddEmployee: Executing AddEmployee Service for Employee Number {request.employeeNumber}");

        try
        {
            // Basic validation of request data
            var validationResult = request.Validate();
            if (validationResult.Count > 0)
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
            }

            // Check tenant exists
            // NOTE: Would have preferred to use TenantService rather than DAL, but hit issue with circular ref on dependency injection
            var readTenantResult = await _tenantActions.Read(tenantId);
            if (!readTenantResult.Item1.Success)
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, $"Invalid Tenant Id {tenantId}");
            }

            // Check uniqueness of key employeeNumber
            var readByEmployeeNumberResult = await _employeeActions.ReadByEmployeeNumber(tenantId, request.employeeNumber);
            if (readByEmployeeNumberResult.Item1.Success)
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "Employee already exists with this Employee Number - must be unique");
            }

            // Check uniqueness of key email
            var readByEmailResult = await _employeeActions.ReadByEmail(tenantId, request.email);
            if (readByEmailResult.Item1.Success)
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "Employee already exists with this Email Address - must be unique");
            }

            // Check leader exists
            if (request.leaderEmployeeNumber.Trim().Length > 0)
            {
                var readByLeaderEmployeeNumberResult = await _employeeActions.ReadByEmployeeNumber(tenantId, request.leaderEmployeeNumber);
                if (!readByLeaderEmployeeNumberResult.Item1.Success)
                {
                    return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, "No employee exists with this Leader Employee Number");
                }
            }

            // Add user to the tenancy of the executing user
            var addUserToTenantResult = await _identityService.AddUserToTenant(readTenantResult.Item2.identityManagerTenantId, request.displayName, request.email, request.phoneNumber, request.initialPassword, requestContext);
            if (!addUserToTenantResult.Success)
            {
                _logger.LogInformation($"EmployeeAddProcessor.AddEmployee: Failed to create user  in GCP Identity Service -  {addUserToTenantResult.Message}");

                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Failed to create user in GCP Identity Service");
            }
            string employeeUID = addUserToTenantResult.uId;

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
                active = request.active,
                adminPrivilege = false
            };

            var saveUserResult = await _employeeActions.Save(tenantId, employeeUID, employeeModel);

            if (saveUserResult.Success)
            {

                var response = new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.Added, "Employee created");
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
                    employeeModel.active,
                    employeeModel.adminPrivilege
                );

                return response;
            }
            else
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Employee could not be added due to unexpected error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeAddProcessor.AddEmployee: Exception caught in AddEmployee service: {ex.Message}");
            return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Unexpected error occurred while adding employee");
        }
    }
}