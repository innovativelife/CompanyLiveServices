using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InnovativeLife.Services.Tenant;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Services.UiShellConfig;
using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Services.Employee;
using InnovativeLife.DataAccess.Employee;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee.Processors;
using InnovativeLife.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using InnovativeLife.Security;
using InnovativeLife.Services.Employee.ServiceMessages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InnovativeLife;

public class Startup : FunctionsStartup
{
    public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
    {
        base.Configure(context, app);

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        DefineEndpoints(app);
    }
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .AddSingleton<IUserContext, UserContext>()
            .AddSingleton<IUiShellConfigService, UiShellConfigService>()
            .AddSingleton<IUiShellConfigActions, UiShellConfigActions>()
            .AddSingleton<IEmployeeService, EmployeeService>()
            .AddSingleton<IEmployeeActions, EmployeeActions>()
            .AddSingleton<IEmployeeAddProcessor, EmployeeAddProcessor>()
            .AddSingleton<IEmployeeReadProcessor, EmployeeReadProcessor>()
            .AddSingleton<IEmployeeSetAdminPrivilegeProcessor, EmployeeSetAdminPrivilegeProcessor>()
            .AddSingleton<IEmployeeSaveProcessor, EmployeeSaveProcessor>()
            .AddSingleton<ITenantActions, TenantActions>()
            .AddSingleton<ITenantService, TenantService>()
            .AddSingleton<ITenantAddProcessor, TenantAddProcessor>()
            .AddSingleton<ITenantReadProcessor, TenantReadProcessor>()
            .AddSingleton<ITenantSaveProcessor, TenantSaveProcessor>()
            .AddSingleton<IMessageService, MessageService>()
            .AddSingleton<IIdentityService, IdentityService>();

        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddAuthentication(GoogleIdentityAuthenticationOptions.DefaultScheme)
            .AddScheme<GoogleIdentityAuthenticationOptions, GoogleIdentityAuthenticationHandler>
                (GoogleIdentityAuthenticationOptions.DefaultScheme,
                options => { });
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperUserRequired", policy => AuthorizationPolicies.GetSuperUserPolicy(policy))
            .AddPolicy("TenantAdmin", policy => AuthorizationPolicies.GetAdminPrivilegePolicy(policy));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void DefineEndpoints(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            {
                addTenantEndpoints(endpoints);
                addEmployeeEndpoints(endpoints);
            });
    }

    private void addTenantEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/tenants/", async (ITenantService service, IUserContext requestContext) =>
            (await service.ReadSet(requestContext)).GetAspNetResult())
        .WithName("TenantReadSet")
        .RequireAuthorization("SuperUserRequired")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get tenants",
            Description = "Get list of tenants that have been configured."
        });

        endpoints.MapGet("/tenants/{tenantId}", async (ITenantService service, IUserContext requestContext, string tenantId) =>
            (await service.Read(requestContext, tenantId)).GetAspNetResult())
        .WithName("TenantRead")
        .RequireAuthorization("SuperUserRequired")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get tenant by ID",
            Description = "Returns details for a single tenant"
        });

        endpoints.MapPost("/tenants", async (ITenantService service, TenantAddRequest addRequest, IUserContext requestContext) =>
            (await service.Add(requestContext, addRequest)).GetAspNetResult())
        .WithName("TenantAdd")
        .RequireAuthorization("SuperUserRequired")
        .Accepts<TenantSaveRequest>("application/json")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add tenant",
            Description = "Add a new tenant."
        });

        endpoints.MapPut("/tenants/{tenantId}", async (ITenantService service, string tenantId, TenantSaveRequest saveRequest, IUserContext requestContext) =>
            (await service.Save(requestContext, tenantId, saveRequest)).GetAspNetResult())
        .WithName("TenantSave")
        .RequireAuthorization("SuperUserRequired")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update tenant",
            Description = "Update the details of an existing tenant."
        });
    }

    private void addEmployeeEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/employees", async (IEmployeeService service, IUserContext requestContext, EmployeeAddRequest addRequest) =>
            (await service.Add(requestContext, addRequest)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("EmployeeAdd")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add employee",
            Description = "Add a new employee to a tenant."
        });

        endpoints.MapGet("/employees/{employeeNumber}", async (IEmployeeService service, IUserContext requestContext, string employeeNumber) =>
            (await service.ReadByEmpoyeeNumber(requestContext, employeeNumber)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("ReadEmployeeByUid")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read employee by employee number",
            Description = "Read employee by employee number"
        });

        endpoints.MapGet("/employees/uid/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string employeeUID) =>
            (await service.ReadByEmployeeUID(requestContext, employeeUID)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("ReadEmployeeByNumber")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read employee by employee uid",
            Description = "Read employee by Employee ID - Guid generated when the employee is created"
        });

        endpoints.MapGet("/employees/emailAddress/{emailAddress}", async (IEmployeeService service, IUserContext requestContext, string emailAddress) =>
            (await service.ReadByEmailAddress(requestContext, emailAddress)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("ReadEmployeeByEmailAddress")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read employee by email address",
            Description = "Read employee by email address"
        });

        endpoints.MapPut("/employees/{employeeUID}/{adminPrivilege}", async (IEmployeeService service, IUserContext requestContext, string employeeUID, bool adminPrivilege) =>
            (await service.SetAdminPrivilege(requestContext, employeeUID, adminPrivilege)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("EmployeeSetAdminPrivilege")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Set admin privilege for an employee",
            Description = "Set admin privilege for an employee. This allows them to perform admin functions within their organisation's tenant."
        });

        endpoints.MapPut("/employees/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string employeeUID, EmployeeSaveRequest saveRequest) =>
            (await service.Save(requestContext, employeeUID, saveRequest)).GetAspNetResult())
        .RequireAuthorization("TenantAdmin")
        .WithName("EmployeeSave")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update employee",
            Description = "Update employee details."
        });
    }
}