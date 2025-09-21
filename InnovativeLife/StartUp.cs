using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InnovativeLife.Services.Tenant;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.Services.UiConfig;
using InnovativeLife.Services.UiConfig.Processors;
using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.DataAccess.UiConfig;
using InnovativeLife.Services.Post;
using InnovativeLife.Services.Post.Processors;
using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.DataAccess.Post;
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
using System.Linq; // Required for .Contains()
using System.Text.RegularExpressions; // Required for Regex


namespace InnovativeLife;

public class Startup : FunctionsStartup
{
    public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
    {
        base.Configure(context, app);

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseCors("_myAllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        DefineEndpoints(app);
    }
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        bool inDevMode = InDevMode();

        services
            .AddScoped<IUserContext, UserContext>()
            .AddSingleton<IUiConfigService, UiConfigService>()
            .AddSingleton<IUiConfigReadProcessor, UiConfigReadProcessor>()
            .AddSingleton<IUiConfigSaveProcessor, UiConfigSaveProcessor>()
            .AddSingleton<IUiConfigActions, UiConfigActions>()
            .AddSingleton<IPostService, PostService>()
            .AddSingleton<IPostReadProcessor, PostReadProcessor>()
            .AddSingleton<IPostSaveProcessor, PostSaveProcessor>()
            .AddSingleton<IPostActions, PostActions>()
            .AddSingleton<IEmployeeService, EmployeeService>()
            .AddSingleton<IEmployeeActions, EmployeeActions>()
            .AddSingleton<IEmployeeAddProcessor, EmployeeAddProcessor>()
            .AddSingleton<IEmployeeReadProcessor, EmployeeReadProcessor>()
            .AddSingleton<IEmployeeSetAdminPrivilegeProcessor, EmployeeSetAdminPrivilegeProcessor>()
            .AddSingleton<IEmployeeSaveProcessor, EmployeeSaveProcessor>()
            .AddSingleton<IEmployeeResetPasswordProcessor, EmployeeResetPasswordProcessor>()
            .AddSingleton<IEmployeeAddFavoriteProcessor, EmployeeAddFavoriteProcessor>()
            .AddSingleton<ITenantActions, TenantActions>()
            .AddSingleton<ITenantService, TenantService>()
            .AddSingleton<ITenantAddProcessor, TenantAddProcessor>()
            .AddSingleton<ITenantReadProcessor, TenantReadProcessor>()
            .AddSingleton<ITenantSaveProcessor, TenantSaveProcessor>()
            .AddSingleton<IMessageService, MessageService>();



        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // services.AddCors(options =>
        // {
        //     options.AddPolicy(name: MyAllowSpecificOrigins,
        //                       policy =>
        //                       {
        //                           policy
        //                             .WithOrigins(
        //                                 "http://localhost", "http://127.0.0.1", "http://localhost:5173", "http://127.0.0.1:5173", // Local development
        //                                 "https://companylive-c3879.web.app", "https://companylive-c3879-pr.web.app/tenant999", "https://companylive-c3879-staging.web.app/tenant999") // Deployed to firebase
        //                             .AllowAnyHeader()
        //                             .AllowAnyMethod();
        //                       });
        // });

        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    // Use SetIsOriginAllowed for custom origin validation
                    policy.SetIsOriginAllowed(origin => IsOriginAllowed(origin))
                        .AllowAnyHeader() // Or .WithHeaders("Content-Type", "Authorization")
                        .AllowAnyMethod(); // Or .WithMethods("GET", "POST", "PUT", "DELETE")
                                           // .AllowCredentials(); // Only if your frontend sends credentials (cookies, auth headers)
                });
        });

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

        AddAuthorisations(services);
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
                addUiConfigEndpoints(endpoints);
                addPostEndpoints(endpoints);
            });
    }

    private void addTenantEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/admin/tenants/", async (ITenantService service, IUserContext requestContext) =>
            (await service.ReadSet(requestContext)).GetAspNetResult())
        .WithName("TenantReadSet")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get tenants",
            Description = "Get list of tenants that have been configured."
        });

        endpoints.MapGet("/api/v1/admin/tenants/{tenantId}", async (ITenantService service, IUserContext requestContext, string tenantId) =>
            (await service.ReadSingleton(requestContext, tenantId)).GetAspNetResult())
        .WithName("TenantRead")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read tenant by ID",
            Description = "Returns details for a single tenant"
        });

        endpoints.MapGet("/api/v1/admin/tenants/{tenantId}/getidentitymanagertenantId", async (ITenantService service, IUserContext requestContext, string tenantId) =>
            (await service.GetIdentityManagerTenantId(requestContext, tenantId)).GetAspNetResult())
        .WithName("TenantGetIdentityManagerTenantId")
        .AllowAnonymous()
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read tenant by ID",
            Description = "Returns details for a single tenant"
        });

        endpoints.MapPost("/api/v1/admin/tenants/", async (ITenantService service, TenantAddRequest addRequest, IUserContext requestContext) =>
            (await service.Add(requestContext, addRequest)).GetAspNetResult())
        .WithName("TenantAdd")
        .RequireAuthorization(AuthorizationPolicies.SuperUserRequired)
        .Accepts<TenantSaveRequest>("application/json")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add tenant",
            Description = "Add a new tenant."
        });

        endpoints.MapPatch("/api/v1/admin/tenants/{tenantId}", async (ITenantService service, string tenantId, TenantSaveRequest saveRequest, IUserContext requestContext) =>
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
        endpoints.MapPost("/api/v1/tenants/{tenantId}/employees", async (IEmployeeService service, IUserContext requestContext, string tenantId, EmployeeAddRequest addRequest) =>
            (await service.Add(requestContext, tenantId, addRequest, false)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeAdd")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add employee",
            Description = "Add a new employee to a tenant."
        });

        endpoints.MapPatch("/api/v1/tenants/{tenantId}/employees/{employeeUID}/admin/{adminPrivilege}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID, bool adminPrivilege) =>
            (await service.SetAdminPrivilege(requestContext, tenantId, employeeUID, adminPrivilege, false)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeSetAdminPrivilege")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Set admin privilege for an employee",
            Description = "Set admin privilege for an employee. This allows them to perform admin functions within their organisation's tenant."
        });

        endpoints.MapPatch("/api/v1/tenants/{tenantId}/employees/{employeeUID}/resetPassword/{newPassword}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID, string newPassword) =>
            (await service.ResetPassword(requestContext, tenantId, employeeUID, newPassword)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeResetPassword")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Reset Password for an employee",
            Description = "Reset password for an employee."
        });

        endpoints.MapPatch("/api/v1/tenants/{tenantId}/employees/addFavorite/{favoriteEmployeeUID}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string favoriteEmployeeUID) =>
            (await service.AddFavorite(requestContext, tenantId, favoriteEmployeeUID)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeAddFavorite")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add employee favorite",
            Description = "Add favorite for the current user"
        });

        endpoints.MapPut("/api/v1/tenants/{tenantId}/employees/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID, EmployeeSaveRequest saveRequest) =>
            (await service.Save(requestContext, tenantId, employeeUID, saveRequest)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("EmployeeUpdate")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update employee",
            Description = "Update existing employee"
        });

        endpoints.MapGet("/api/v1/tenants/{tenantId}/employees/{employeeUID}", async (IEmployeeService service, IUserContext requestContext, string tenantId, string employeeUID) =>
            (await service.ReadByEmployeeUID(requestContext, tenantId, employeeUID, false)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
        .WithName("EmployeeReadByUID")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Read employee by employee uid",
            Description = "Read employee by Employee UID - Guid generated when the employee is created"
        });

        endpoints.MapGet("/api/v1/tenants/{tenantId}/employees", async (IEmployeeService service, IUserContext requestContext, string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber, string? employeeUID) =>
            (await service.SearchEmployee(requestContext, tenantId, employeeNumber, email, firstName, lastName, leaderEmployeeNumber, employeeUID)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
        .WithName("EmployeeSearch")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Employee Search",
            Description = "Search for employees via various criteria"
        });
    }

    private void addUiConfigEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/tenants/{tenantId}/uiconfig",
          async (IUiConfigService service, IUserContext requestContext, string tenantId) =>
             (await service.Read(requestContext, tenantId)).GetAspNetResult())
       .AllowAnonymous()
       .WithName("ReadUiConfig")
       .WithOpenApi(operation => new(operation)
       {
           Summary = "Read UI config for tennant",
           Description = "Read UI Config by tenant ID"
       });

        endpoints.MapPost("/api/v1/tenants/{tenantId}/uiconfig",
          async (IUiConfigService service, IUserContext requestContext, UiConfigSaveRequest saveRequest, string tenantId) =>
             (await service.Save(requestContext, tenantId, saveRequest)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("Save UiConfig")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Save UI config for tennant",
            Description = "Save UI Config by tenant ID"
        });
    }

    private void addPostEndpoints(IEndpointRouteBuilder endpoints)
    {
        //     endpoints.MapGet("/post/{tenantId}",
        //       async (IPostService service, IUserContext requestContext, string tenantId, string postId) =>
        //          (await service.Read(requestContext, tenantId, postId)).GetAspNetResult())
        //    .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
        //    .WithName("ReadPost")
        //    .WithOpenApi(operation => new(operation)
        //    {
        //        Summary = "Read Post by user",
        //        Description = "Read Post by user and tenant ID"
        //    });

        endpoints.MapPost("/api/v1/tenants/{tenantId}/post",
          async (IPostService service, IUserContext requestContext, PostSaveRequest saveRequest, string tenantId) =>
             (await service.Save(requestContext, tenantId, saveRequest)).GetAspNetResult())
        // .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("Save Post")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Save Post by user",
            Description = "Save Post by user and tenant ID"
        });

        endpoints.MapGet("/api/v1/tenants/{tenantId}/post", async (IPostService service, IUserContext requestContext, string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message) =>
             (await service.SearchPost(requestContext, tenantId, postId, timeSent, sendTo, status, employeeUID, message)).GetAspNetResult())
         .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
         .WithName("PostSearch")
         .WithOpenApi(operation => new(operation)
         {
             Summary = "Search for posts",
             Description = "Search for posts via various criteria"
         });

        endpoints.MapPost("/api/v1/tenants/{tenantId}/{postId}/reply", async (IPostService service, IUserContext requestContext, PostAddReplyResquest saveRequest, string tenantId, string postId) =>
            (await service.AddPostReply(requestContext, tenantId, postId, saveRequest)).GetAspNetResult())
        .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
        .WithName("Save Reply")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Save Reply by user",
            Description = "Save Reply by reply, user and tenant ID"
        });

        endpoints.MapGet("/api/v1/tenants/{tenantId}/{postId}/replies", async (IPostService service, IUserContext requestContext, string tenantId, string postId) =>
             (await service.ReadReplies(requestContext, tenantId, postId)).GetAspNetResult())
         .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
         .WithName("Post Reply Search")
         .WithOpenApi(operation => new(operation)
         {
             Summary = "Search for replies",
             Description = "Search for replies via various criteria"
         });

        endpoints.MapPost("/api/v1/tenants/{tenantId}/{postId}/react", async (IPostService service, IUserContext requestContext, PostAddReactionResquest saveRequest, string tenantId, string postId) =>
           (await service.AddPostReaction(requestContext, tenantId, postId, saveRequest)).GetAspNetResult())
       .RequireAuthorization(AuthorizationPolicies.TenantAdmin)
       .WithName("Save Reaction")
       .WithOpenApi(operation => new(operation)
       {
           Summary = "Save Raction by user",
           Description = "Save Reaction by reaction, user and tenant ID"
       });

        endpoints.MapGet("/api/v1/tenants/{tenantId}/{postId}/react", async (IPostService service, IUserContext requestContext, string tenantId, string postId) =>
             (await service.ReadReactions(requestContext, tenantId, postId)).GetAspNetResult())
         .RequireAuthorization(AuthorizationPolicies.GetTenantUserPolicy)
         .WithName("Post Reaction Search")
         .WithOpenApi(operation => new(operation)
         {
             Summary = "Search for reactions",
             Description = "Search for reactions via various criteria"
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

    private bool IsOriginAllowed(string origin)
    {
        // Define exact allowed origins
        var allowedExactOrigins = new[]
        {
            "http://localhost", // Local development
            "http://127.0.0.1", // Local development
            "http://localhost:5173", // Local development
            "http://127.0.0.1:5173", // Local development
            "https://companylive-c3879.web.app",  // Production URL
            "https://companylive-staging-sydney.web.app"  // Staging URL
        };

        // Check if the origin is in the exact allowed list
        if (allowedExactOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // Define Regex for dynamic Firebase Hosting Preview and PR Channels
        // This regex allows anything between the project ID and ".web.app" as long as it's preceded by "--"
        // It covers:
        // - https://companylive-c3879--pr-123.web.app (if you change your GitHub Action to use PR number)
        // - https://companylive-c3879--staging-o6rf2eif.web.app (your current observed format)
        // - https://companylive-c3879--any-custom-channel-name.web.app
        var firebasePreviewRegex = new Regex(@"^https:\/\/companylive-pr-[a-zA-Z0-9-]+\.web\.app$", RegexOptions.IgnoreCase);

        // Check if the origin matches the preview channel pattern
        if (firebasePreviewRegex.IsMatch(origin))
        {
            return true;
        }

        // No match - the origin is not allowed
        return false;
    }
}