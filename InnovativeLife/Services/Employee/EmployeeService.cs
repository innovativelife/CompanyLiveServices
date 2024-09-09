using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee.ServiceMessages;
using InnovativeLife.Services.Employee.Processors;
using System.Runtime.CompilerServices;

namespace InnovativeLife.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private ILogger<EmployeeService> _logger;
    private IEmployeeAddProcessor _employeeCreateProcessor;
    private IEmployeeSetAdminPrivilegeProcessor _employeeSetAdminPrivilegeProcessor;
    private IEmployeeReadProcessor _employeeReadProcessor;
    private IEmployeeSaveProcessor _employeeSaveProcessor;
    private IIdentityService _identityService;

    public EmployeeService(ILogger<EmployeeService> logger, IIdentityService identityService, IEmployeeAddProcessor employeeCreateProcessor, IEmployeeSetAdminPrivilegeProcessor employeeSetAdminPrivilegeProcessor, IEmployeeReadProcessor employeeReadProcessor, IEmployeeSaveProcessor employeeSaveProcessor)
    {
        _logger = logger;
        _identityService = identityService;
        _employeeCreateProcessor = employeeCreateProcessor;
        _employeeSetAdminPrivilegeProcessor = employeeSetAdminPrivilegeProcessor;
        _employeeSaveProcessor = employeeSaveProcessor;
        _employeeReadProcessor = employeeReadProcessor;
    }

    public async Task<EmployeeAddResponse> Add(IUserContext requestContext, EmployeeAddRequest request)
    {
        return await _employeeCreateProcessor.AddEmployee(requestContext, request);
    }

    public async Task<EmployeeSetAdminPrivilegeResponse> SetAdminPrivilege(IUserContext requestContext, string employeeUID, bool adminPrivilege)
    {
        return await _employeeSetAdminPrivilegeProcessor.SetAdminPrivilege(requestContext, employeeUID, adminPrivilege);
    }

    public async Task<EmployeeReadResponse> ReadByEmployeeUID(IUserContext requestContext, string employeeUID)
    {
        return await _employeeReadProcessor.ReadByEmployeeUID(requestContext, employeeUID);
    }
    
    public async Task<EmployeeSaveResponse> Save(IUserContext requestContext, string employeeUID, EmployeeSaveRequest request)
    {
        return await _employeeSaveProcessor.SaveEmployee(requestContext, employeeUID, request);
    }

    public async Task<EmployeeSearchResponse> SearchEmployee(IUserContext requestContext, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber)
    {
        return await _employeeReadProcessor.SearchEmployee(requestContext, employeeNumber, email, firstName, lastName, leaderEmployeeNumber);
    }
}