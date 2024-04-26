using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InnovativeLife.CloudFunctionHandler;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Tenant;
using InnovativeLife.Services.Tenant.Processors;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Services.UiShellConfig;
using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Services.User;
using InnovativeLife.DataAccess.User;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.User.Processors;
using InnovativeLife.WebApi.Common;

namespace InnovativeLife;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
        services
            .AddSingleton<IRouter, Router>()
            .AddSingleton<IIdentityService, IdentityService>()
            .AddSingleton<IUiShellConfigHandler, UiShellConfigHandler>()
            .AddSingleton<IUiShellConfigService, UiShellConfigService>()
            .AddSingleton<IUiShellConfigActions, UiShellConfigActions>()
            .AddSingleton<IUserHandler, UserHandler>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserActions, UserActions>()
            .AddSingleton<IUserCreateProcessor, UserCreateProcessor>()
            .AddSingleton<ITenantHandler, TenantHandler>()
            .AddSingleton<ITenantActions, TenantActions>()
            .AddSingleton<ITenantService, TenantService>()
            .AddSingleton<ITenantAddProcessor, TenantAddProcessor>()
            .AddSingleton<ITenantReadProcessor, TenantReadProcessor>()
            .AddSingleton<ITenantSaveProcessor, TenantSaveProcessor>();
}