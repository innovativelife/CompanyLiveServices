using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InnovativeLife.CloudFunctionHandler;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using InnovativeLife.Common;
using InnovativeLife.Services.Employee.ServiceMessages;

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
            .AddSingleton<IAuthorizationHandler, AuthorizationRequirementHandler>()
            .AddSingleton<IRequestContext, RequestContext>()
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

        services.AddAuthentication("GoogleIdentityPlatform")
            .AddScheme<SimpleOptions, SimpleAuthHandler>("from startup", o => 
                {
                    o.DisplayMessage = "************************** Hello from statup";
                    o.ForwardDefaultSelector = ctx =>
                        ctx.Request.Path.StartsWithSegments("/") ? "/" : null;
                });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", p => p.AddRequirements(new AuthorizationRequirement("Admin")));
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void DefineEndpoints(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/tenant/read/{tenantId}", async (ITenantService service, IRequestContext requestContext, string tenantId) =>
                    await service.Read(requestContext, tenantId)).RequireAuthorization("Admin")
                .WithName("TenantRead")
                .WithOpenApi();

                endpoints.MapPost("/tenant/add", async (ITenantService service, TenantAddRequest addRequest, IRequestContext requestContext) => 
                    await service.Add(requestContext, addRequest)).RequireAuthorization("Admin")
                .WithName("TenantAdd")
                .WithOpenApi();

                endpoints.MapPost("/employee/add", async (IEmployeeService service, IRequestContext requestContext, EmployeeAddRequest addRequest) =>
                    await service.AddEmployee(requestContext, addRequest)).RequireAuthorization("Admin")
                .WithName("EmployeeAdd")
                .WithOpenApi();
            });
    }
}