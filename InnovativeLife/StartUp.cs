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
            .AddSingleton<IMessageService, MessageService>()
            .AddSingleton<IIdentityService, IdentityService>()
            .AddSingleton<IUserContext, UserContext>()
            .AddSingleton<IUiShellConfigService, UiShellConfigService>()
            .AddSingleton<IUiShellConfigActions, UiShellConfigActions>()
            .AddSingleton<IEmployeeService, EmployeeService>()
            .AddSingleton<IEmployeeActions, EmployeeActions>()
            .AddSingleton<IEmployeeAddProcessor, EmployeeAddProcessor>()
            .AddSingleton<ITenantActions, TenantActions>()
            .AddSingleton<ITenantService, TenantService>()
            .AddSingleton<ITenantAddProcessor, TenantAddProcessor>()
            .AddSingleton<ITenantReadProcessor, TenantReadProcessor>()
            .AddSingleton<ITenantSaveProcessor, TenantSaveProcessor>();

        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddAuthentication(GoogleIdentityAuthenticationOptions.DefaultScheme)
            .AddScheme<GoogleIdentityAuthenticationOptions, GoogleIdentityAuthenticationHandler>
                (GoogleIdentityAuthenticationOptions.DefaultScheme,
                options => { });
        services.AddAuthorizationBuilder()
            .AddPolicy("SuperUserRequired", policy => AuthorizationPolicies.GetSuperUserPolicy(policy));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void DefineEndpoints(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/tenants/", async (ITenantService service, IUserContext requestContext) =>
                    await service.ReadSet(requestContext))
                .WithName("TenantReadSet")
                .RequireAuthorization("SuperUserRequired")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Get tenants",
                    Description = "Get list of tenants that have been configured."
                });

                endpoints.MapGet("/tenants/{tenantId}", async (ITenantService service, IUserContext requestContext, string tenantId) =>
                    await service.Read(requestContext, tenantId))
                .WithName("TenantRead")
                .RequireAuthorization("SuperUserRequired")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Get tenant by ID",
                    Description = "Returns details for a single tenant"
                });

                endpoints.MapPost("/tenants", async (ITenantService service, TenantAddRequest addRequest, IUserContext requestContext) =>
                    await service.Add(requestContext, addRequest))
                .WithName("TenantAdd")
                .RequireAuthorization("SuperUserRequired")
                .Accepts<TenantSaveRequest>("application/json")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Add tenant",
                    Description = "Add a new tenant."
                });

                endpoints.MapPut("/tenants/{tenantId}", async (ITenantService service, string tenantId, TenantSaveRequest saveRequest, IUserContext requestContext) =>
                    await service.Save(requestContext, tenantId, saveRequest))
                .WithName("TenantSave")
                .RequireAuthorization("SuperUserRequired")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Update tenant",
                    Description = "Update the details of an existing tenant."
                });

                endpoints.MapPost("/employees", async (IEmployeeService service, IUserContext requestContext, EmployeeAddRequest addRequest) =>
                    await service.AddEmployee(requestContext, addRequest))
                .RequireAuthorization("SuperUserRequired")
                .WithName("EmployeeAdd")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Add employee",
                    Description = "Add a new employee to a tenant."
                });
            });
    }
}