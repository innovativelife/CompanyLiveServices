using Microsoft.Extensions.Logging;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Localization;
using InnovativeLife.Services.Tenant;
using InnovativeLife.DataAccess.Tenant;

namespace InnovativeLife.Services.Employee.Processors;

public class EmployeeAddFavoriteProcessor : IEmployeeAddFavoriteProcessor
{
    private readonly ILogger<IEmployeeAddProcessor> _logger;
    private readonly IMessageService _messageService;
    private readonly IIdentityService _identityService;
    private readonly IEmployeeActions _employeeActions;
    private readonly ITenantActions _tenantActions;

    public EmployeeAddFavoriteProcessor(ILogger<IEmployeeAddProcessor> logger, IMessageService messageService, IIdentityService identityService, IEmployeeActions employeeActions, ITenantActions tenantActions)
    {
        _logger = logger;
        _messageService = messageService;
        _identityService = identityService;
        _employeeActions = employeeActions;
        _tenantActions = tenantActions;
    }

    public async Task<EmployeeAddFavoriteResponse> EmployeeAddFavoriteEmployee(IUserContext requestContext, string tenantId, string employeeUID, string favoriteEmployeeUId)
    {
        _logger.LogInformation($"EmployeeAddProcessor.AddFavorite: Executing AddFavorite Service for user {requestContext.uId}");

        try
        {
            // Validate that the favorite employee exists
            var readFavoriteEmployee = await _employeeActions.ReadByEmployeeUID(tenantId, favoriteEmployeeUId);
            if (!readFavoriteEmployee.Item1.Success)
            {
                new EmployeeAddFavoriteResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, $"Favorited employee {favoriteEmployeeUId} not found");
            }

            // Add favorite
            var result = await _employeeActions.AddFavorite(tenantId, employeeUID, favoriteEmployeeUId);

            if (result.Success)
            {
                return new EmployeeAddFavoriteResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Favorite added");
            }

            return  new EmployeeAddFavoriteResponse(Common.ServiceResponseBase.ResponseStatus.BusinessError, "Favorite could not be added due to unexpected error");

        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeAddProcessor.AddEmployee: Exception caught in AddFavorite service: {ex.Message}");
            return new EmployeeAddFavoriteResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Unexpected error occurred while adding favorite for employee");
        }
    }
}