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
        bool inDevMode = InDevMode();

        services
            .AddScoped<IUserContext, UserContext>()
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
            .AddSingleton<IMessageService, MessageService>();



        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        if (inDevMode)
        {
            SetUpLocalDevAuth(services);
        }
        else
        {
            SetUpGoogleAuth(services);
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void SetUpGoogleAuth(IServiceCollection services)
    {
        services.AddSingleton<IIdentityService, IdentityService>();

        services.AddAuthentication(GoogleIdentityAuthenticationOptions.DefaultScheme)
            .AddScheme<GoogleIdentityAuthenticationOptions, GoogleIdentityAuthenticationHandler>
                (GoogleIdentityAuthenticationOptions.DefaultScheme,
                options => { });
       
        services.AddAuthentication();
    }

    private void SetUpLocalDevAuth(IServiceCollection services)
    {
        services.AddSingleton<IIdentityService, LocalDevIdentityService>();

        services.AddAuthentication(GoogleIdentityAuthenticationOptions.DefaultScheme)
            .AddScheme<GoogleIdentityAuthenticationOptions, LocalDevAuthenticationHandler>
                (GoogleIdentityAuthenticationOptions.DefaultScheme,
                options => { });

        AddAuthorisations(services);
    }

    private void AddAuthorisations(IServiceCollection services)
    {
          services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.SuperUserRequired, policy => AuthorizationPolicies.GetSuperUserPolicy(policy))
            .AddPolicy(AuthorizationPolicies.TenantAdmin, policy => AuthorizationPolicies.GetTenantAdminPolicy(policy))
            .AddPolicy(AuthorizationPolicies.TenantUser, policy => AuthorizationPolicies.GetTenantUserPolicy(policy));
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
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get tenants",
            Description = "Get list of tenants that have been configured."
        });

        endpoints.MapGet("/tenants/{tenantId}", async (ITenantService service, IUserContext requestContext, string tenantId) =>
            (await service.ReadSingleton(requestContext, tenantId)).GetAspNetResult())
        .WithName("TenantRead")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get tenant by ID",
            Description = "Returns details for a single tenant"
        });

        endpoints.MapPost("/tenants/", async (ITenantService service, TenantAddRequest addRequest, IUserContext requestContext) =>
            (await service.Add(requestContext, addRequest)).GetAspNetResult())
        .WithName("TenantAdd")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .Accepts<TenantSaveRequest>("application/json")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add tenant",
            Description = "Add a new tenant."
        });

        endpoints.MapPut("/tenants/{tenantId}", async (ITenantService service, string tenantId, TenantSaveRequest saveRequest, IUserContext requestContext) =>
            (await service.Save(requestContext, tenantId, saveRequest)).GetAspNetResult())
        .WithName("TenantSave")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update tenant",
            Description = "Update the details of an existing tenant."
        });
    }

    private void addEmployeeEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/employees/{tenantId}", async (IEmployeeService service, IUserContext requestContext, string tenantId, EmployeeAddRequest addRequest) =>
            (await service.Add(requestContext, tenantId, addRequest)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeAdd")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add employee",
            Description = "Add a new employee to a tenant."
        });

        endpoints.MapPut("/employees/{tenantId}/{employeeUID}/admin/{adminPrivilege}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID, bool adminPrivilege) =>
            (await service.SetAdminPrivilege(requestContext, tenantId, employeeUID, adminPrivilege)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeSetAdminPrivilege")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Set admin privilege for an employee",
            Description = "Set admin privilege for an employee. This allows them to perform admin functions within their organisation's tenant."
        });

        endpoints.MapPut("/employees/{tenantId}/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest saveRequest) =>
            (await service.Save(requestContext, tenantId, employeeUID, saveRequest)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeSave")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update employee",
            Description = "Update employee details."
        });

        endpoints.MapGet("/employees/{tenantId}/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID) =>
            (await service.ReadByEmployeeUID(requestContext, tenantId, employeeUID)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
        .WithName("ReadEmployeeByNumber")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read employee by employee uid",
            Description = "Read employee by Employee ID - Guid generated when the employee is created"
        });

        endpoints.MapGet("/employees/{tenantId}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber) =>
            (await service.SearchEmployee(requestContext, tenantId, employeeNumber, email, firstName, lastName, leaderEmployeeNumber)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
        .WithName("EmployeeSearch")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Search for employees",
            Description = "Search for employees via various criteria"
        });
    }


    private bool InDevMode()
    {
        // Determine if executing in development mode
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        var devMode = env != null && env.ToLower() == "development";
        if (devMode)
        {
            // _logger.LogWarning("GoogleIdentityAuthenticationHandler.InDevMode: In Dev Mode");
        }
        return devMode;
    }
}