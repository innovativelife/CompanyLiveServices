using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Common;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeAddProcessor : IEmployeeAddProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IIdentityService _identityService;
    private readonly IEmployeeActions _employeeActions;

    public EmployeeAddProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IIdentityService identityService, IEmployeeActions employeeActions)
    {
        _logger = logger;
        _messageService = messageService;
        _identityService = identityService;
        _employeeActions = employeeActions;
    }

    public async Task<EmployeeAddResponse> AddEmployee(IRequestContext requestContext, EmployeeAddRequest request)
    {
        _logger.LogInformation("Executing CreateUser Service");

        try
        {
            // Validate  Tenant ID is not blank
            if (string.IsNullOrWhiteSpace(request.tenantId))
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, _messageService.GetMessage("Tenant_Id_Mandatory"));
            }

            // Validate  firstName is not blank
            if (string.IsNullOrWhiteSpace(request.firstName))
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, _messageService.GetMessage("First_Name_Mandatory"));
            }

            // Validate  lastName is not blank
            if (string.IsNullOrWhiteSpace(request.lastName))
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, _messageService.GetMessage("Last_Name_Mandatory"));
            }

            string EmployeeUID;

            if (requestContext.developmentMode)
            {
                _logger.LogInformation("Development mode - User creation skipped");
                EmployeeUID = Guid.NewGuid().ToString();
            }
            else
            {
                // Add user to the tenancy of the executing user
                var addUserToTenantResult = await _identityService.AddUserToTenant(requestContext.tenantId, request.displayName, request.email, request.phoneNumber, request.initialPassword);
                if (!addUserToTenantResult.Success)
                {
                    return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Failed to create user: {addUserToTenantResult.message}");
                }
                EmployeeUID = addUserToTenantResult.uId;
            }

            // Create the employee for user in the DB
            var employeeModel = new DataAccess.Employee.Employee
            {
                active = true,
                firstName = request.firstName,
                lastName = request.lastName,
                preferredName = request.preferredName,
                userUID = EmployeeUID,
                email = request.email,
                phoneNumber = request.phoneNumber,
                tenantAdmin = request.tenantAdmin
            };

            var saveUserResult = await _employeeActions.Save(EmployeeUID, employeeModel);

            if (saveUserResult.Item1.Success)
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "User created");
            }
            else
            {
                return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "User could not be added due to unexpected DB error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught in CreateUser service: {ex.Message}");
            return new EmployeeAddResponse(Common.ServiceResponseBase.ResponseStatus.Exception, ex.Message);
        }
    }
}