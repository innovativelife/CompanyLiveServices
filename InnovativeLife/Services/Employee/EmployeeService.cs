using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Services.Employee.Processors;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private ILogger<EmployeeService> _logger;
    private IEmployeeAddProcessor _employeeCreateProcessor;
    private IEmployeeSetAdminPrivilegeProcessor _employeeSetAdminPrivilegeProcessor;
    private IEmployeeReadProcessor _employeeReadProcessor;
    private IEmployeeSaveProcessor _employeeSaveProcessor;
    private IIdentityService _identityService;
    private IEmployeeResetPasswordProcessor _employeeResetPasswordProcessor;

    public EmployeeService(ILogger<EmployeeService> logger, IIdentityService identityService, IEmployeeAddProcessor employeeCreateProcessor, IEmployeeSetAdminPrivilegeProcessor employeeSetAdminPrivilegeProcessor, IEmployeeReadProcessor employeeReadProcessor, IEmployeeSaveProcessor employeeSaveProcessor, IEmployeeResetPasswordProcessor employeeResetPasswordProcessor)
    {
        _logger = logger;
        _identityService = identityService;
        _employeeCreateProcessor = employeeCreateProcessor;
        _employeeSetAdminPrivilegeProcessor = employeeSetAdminPrivilegeProcessor;
        _employeeSaveProcessor = employeeSaveProcessor;
        _employeeReadProcessor = employeeReadProcessor;
        _employeeSaveProcessor = employeeSaveProcessor;
        _employeeResetPasswordProcessor = employeeResetPasswordProcessor;
    }

    public async Task<EmployeeAddResponse> Add(IUserContext requestContext, string tenantId, EmployeeAddRequest request, bool allowRoot)
    {
        var validation = validateTenantId(requestContext, tenantId, allowRoot);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeAddResponse(validation.Item1, validation.Item2);
        }

        return await _employeeCreateProcessor.AddEmployee(requestContext, tenantId, request);
    }

    public async Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string tenantId, string employeeUID, bool adminPrivilege, bool allowRoot)
    {
        var validation = validateTenantId(requestContext, tenantId, allowRoot);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeSetAdminPrivilegeResponse(validation.Item1, validation.Item2);
        }

        return await _employeeSetAdminPrivilegeProcessor.SetAdminPrivilege(requestContext, tenantId, employeeUID, adminPrivilege);
    }

    public async Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string tenantId, string employeeUID, bool allowRoot)
    {
       var validation = validateTenantId(requestContext, tenantId, allowRoot);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeReadResponse(validation.Item1, validation.Item2);
        }

        return await _employeeReadProcessor.ReadByEmployeeUID(requestContext, tenantId, employeeUID);
    }

    public async Task<EmployeeSaveResponse> Save(IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest request)
    {
        var validation = validateTenantId(requestContext, tenantId, false);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeSaveResponse(validation.Item1, validation.Item2);
        }

        return await _employeeSaveProcessor.SaveEmployee(requestContext, tenantId, employeeUID, request);
    }

    public async Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber)
    {
        var validation = validateTenantId(requestContext, tenantId, false);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeSearchResponse(validation.Item1, validation.Item2);
        }

        return await _employeeReadProcessor.SearchEmployee(requestContext, tenantId, employeeNumber, email, firstName, lastName, leaderEmployeeNumber);
    }

    public async Task<EmployeeResetPasswordResponse> ResetPassword(IUserContext requestContext, string tenantId, string employeeUID, string newPassword)
    {
        var validation = validateTenantId(requestContext, tenantId, false);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new EmployeeResetPasswordResponse(validation.Item1, validation.Item2);
        }

        return await _employeeResetPasswordProcessor.ResetPassword(requestContext, tenantId, employeeUID, newPassword);
    }

    private Tuple<ServiceResponseBase.ResponseStatus, string> validateTenantId(IUserContext requestContext, string tenantId, bool allowRoot)
    {
        if (allowRoot && requestContext.rootAdmin)
        {
            // operation permitted - this is to enable Adding admin employees when creating new tenant
        }
        else if (!string.Equals(requestContext.tenantId, tenantId))
        {
            return new Tuple<ServiceResponseBase.ResponseStatus, string>(ServiceResponseBase.ResponseStatus.BadRequest, "Tenant mismatch");
        }

        return new Tuple<ServiceResponseBase.ResponseStatus, string>(ServiceResponseBase.ResponseStatus.Ok, "");
    }
}