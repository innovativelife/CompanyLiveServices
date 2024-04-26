using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using InnovativeLife.WebApi;
using InnovativeLife.DataAccess.User;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Common;

namespace InnovativeLife.CloudFunctionHandler;

[FunctionsStartup(typeof(Startup))]
public class RequestHandler : IHttpFunction
{
    private readonly ILogger<RequestHandler> _logger;
    private readonly IRouter _router;
    private readonly IIdentityService _identityService;

    private readonly bool _isDevelopmentMode;

    // Initialise Request Handler, setting up Routes for each virtual end point
    public RequestHandler(
        ILogger<RequestHandler> logger,
        IRouter router,
        IIdentityService identityService,
        IUiShellConfigHandler configHandler,
        IUserHandler userHandler,
        ITenantHandler tenantHandler)
    {
        _logger = logger;
        _router = router;
        _identityService = identityService;

        // Determine if executing in development mode
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        _isDevelopmentMode = env != null && env.ToLower() == "development";

        if (_isDevelopmentMode)
            _logger.LogWarning("Executing in Development Mode");

        // Register Routes --- Ideally his would not be required - can we search the services registered via DI?
        router.RegisterRoute("GET", "UiShellConfig", configHandler);
        router.RegisterRoute("POST", "UiShellConfig", configHandler);
        router.RegisterRoute("GET", "Tenant", tenantHandler);
        router.RegisterRoute("POST", "Tenant", tenantHandler);
        router.RegisterRoute("GET", "User", userHandler);
        router.RegisterRoute("POST", "User", userHandler);
    }

    /// <summary>
    /// Process cloud function request
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {
        try
        {

            _logger.LogInformation($"Method: {context.Request.Method}\n");
            _logger.LogInformation($"Path: {context.Request.Path}\n");

            var authResult = await AuthoriseRequest(context);
            if (authResult.Item1)
            {
                // Route request and return response
                var response = await _router.RouteRequest(context, authResult.Item2);
                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(response.ResponseData);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Not Authorised");
            }

            _logger.LogInformation($"Processing Complete\n");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message}: \n{ex.StackTrace}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }

    // Authorise request by:
    // - Using bearer token to validate request and identify user
    // - Using tenantId in header to validate Tenant and ensure user is member of tenant
    private async Task<Tuple<bool, RequestContext?>> AuthoriseRequest(HttpContext httpContext)
    {
        var requestContext = new RequestContext();

        _logger.LogInformation("Authorising request");
        if (_isDevelopmentMode)
        {
            _logger.LogInformation("Skipping authorisation in development mode");
            requestContext.SetDevelopmentModeContext();
            return new Tuple<bool, RequestContext?>(true, requestContext);
        }

        var authToken = GetAuthTokenFromHeader(httpContext);
        if (!authToken.Item1)
        {
            return new Tuple<bool, RequestContext?>(false, requestContext);
        }

        var tenant = GetTenantFromHeader(httpContext);
        if (!tenant.Item1)
        {
            return new Tuple<bool, RequestContext?>(false, requestContext);
        }

        var validateAuthResult = await _identityService.AuthenticateUserAndTenant(authToken.Item2!, tenant.Item2!);
        if (!validateAuthResult.Item1)
        {
            return new Tuple<bool, RequestContext?>(false, requestContext);
        }

        _logger.LogInformation($"Authorization completed successfully for user {validateAuthResult.Item2!.uId} to access tenant {validateAuthResult.Item2.tenantId}");

        _logger.LogInformation("Setting user context");

        return new Tuple<bool, RequestContext?>(true, validateAuthResult.Item2);
    }

    // Extract the bearer token from the HTTP header
    private Tuple<bool, string?> GetAuthTokenFromHeader(HttpContext httpContext)
    {
        _logger.LogInformation("About to get Auth Token");
        if (!httpContext.Request.Headers.ContainsKey("Authorization"))
        {
            _logger.LogWarning("Authorization token not found in header");
            return new Tuple<bool, string?>(false, "");
        }

        var authorization = httpContext.Request.Headers["Authorization"].ToString();
        var tokenComponents = authorization.Split("Bearer ");

        if (tokenComponents.Length == 0)
        {
            _logger.LogWarning("Invalid format of bearer token");
            return new Tuple<bool, string?>(false, "");
        }

        var token = tokenComponents[1];
        _logger.LogInformation($"Token Length: {token.Length}");

        return new Tuple<bool, string?>(true, token);
    }

    // Extract the tenant from the Http Header.  This is a customer attribute required for many requests of the CompanyLive Services.
    private Tuple<bool, string?> GetTenantFromHeader(HttpContext httpContext)
    {
        _logger.LogInformation("About to validate tenant Id");
        if (!httpContext.Request.Headers.ContainsKey("tenantId"))
        {
            _logger.LogInformation("tenantId not included in header");
            return new Tuple<bool, string?>(false, ""); ;
        }

        var tenantId = httpContext.Request.Headers["tenantId"].ToString();
        _logger.LogInformation($"tenantId from header is {tenantId}");

        return new Tuple<bool, string?>(true, tenantId);
    }

    // // ToDo:
    // // - Populate the user context, including user and tenant details
    // private async Task<Tuple<bool, UserModel?>> GetUserModel(string uid, string tenantId)
    // {
    //     _logger.LogInformation($"About to read User record for uid: {uid}");
    //     var readUserResult = await _userActions.ReadByUID(uid);
    //     if (!readUserResult.Item1.Success)
    //     {
    //         _logger.LogInformation($"uid {uid} not found in user collection");
    //         return new Tuple<bool, UserModel?>(false, null);
    //     }

    //     if (readUserResult.Item2 == null)
    //     {
    //         _logger.LogInformation("No user returned from user read, although read returned true?");
    //     }

    //     _logger.LogInformation($"uid {uid} found in user collection for user {readUserResult.Item2!.identifier}");

    //     if (readUserResult.Item2!.tenantAccessList is null || readUserResult.Item2.tenantAccessList.Count == 0)
    //     {
    //         _logger.LogInformation($"uid {uid} has no tenants");
    //         return new Tuple<bool, UserModel?>(false, null);
    //     }

    //     _logger.LogInformation($"About to search tenant collection for tenant {tenantId}");
    //     bool tenantFoundForUser = false;
    //     foreach (var tenant in readUserResult.Item2.tenantAccessList)
    //     {
    //         if (tenant.tenantId.Equals(tenantId) && tenant.active)
    //         {
    //             tenantFoundForUser = true;
    //             _logger.LogInformation($"Active tenant {tenantId} found for user");
    //         }
    //     }

    //     if (tenantFoundForUser)
    //     {
    //         _logger.LogInformation($"Active instance of tenant {tenantId} found for use");
    //         return new Tuple<bool, UserModel?>(true, readUserResult.Item2);
    //     }

    //     _logger.LogInformation("User does not have access to tenant Id");
    //     return new Tuple<bool, UserModel?>(false, null);
    // }
}