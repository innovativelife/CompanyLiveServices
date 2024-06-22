using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using InnovativeLife.Localization;
using InnovativeLife.GcpServices.Identity;
using Microsoft.Extensions.Logging;
using InnovativeLife.Common;


namespace InnovativeLife.CloudFunctionHandler;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public AuthorizationRequirement(string role) => Role = role;
    public string Role { get; set; }
}

public class AuthorizationRequirementHandler : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly ILogger<AuthorizationRequirementHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestContext _requestContext;
    private readonly IIdentityService _identityService;
    private readonly IMessageService _messageService;
    private readonly bool _isDevelopmentMode;

    public AuthorizationRequirementHandler(ILogger<AuthorizationRequirementHandler> logger, IHttpContextAccessor httpContextAccessor, IRequestContext requestContext, IIdentityService identityService, IMessageService messageService)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _requestContext = requestContext;
        _identityService = identityService;
        _messageService = messageService;

        // Determine if executing in development mode
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        _isDevelopmentMode = env != null && env.ToLower() == "development";
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        _logger.LogInformation("Authorisation of request started");

        if (_isDevelopmentMode)
            _logger.LogWarning("Executing in Development Mode");

        var authResult = await AuthorizeRequest(_httpContextAccessor.HttpContext);

        if (authResult.Item1)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private async Task<Tuple<bool, IRequestContext?>> AuthorizeRequest(HttpContext? httpContext)
    {
        _logger.LogInformation("Authorising request");
        if (_isDevelopmentMode)
        {
            _logger.LogInformation("Skipping authorisation in development mode");
            _requestContext.SetDevelopmentModeContext();
            return new Tuple<bool, IRequestContext?>(true, _requestContext);
        }

        var authToken = GetAuthTokenFromHeader(httpContext);
        if (!authToken.Item1)
        {
            return new Tuple<bool, IRequestContext?>(false, _requestContext);
        }

        var tenant = GetTenantFromHeader(httpContext);
        if (!tenant.Item1)
        {
            return new Tuple<bool, IRequestContext?>(false, _requestContext);
        }

        var validateAuthResult = await _identityService.AuthenticateUserAndTenant(authToken.Item2!, tenant.Item2!);
        if (!validateAuthResult.Item1)
        {
            return new Tuple<bool, IRequestContext?>(false, _requestContext);
        }

        _logger.LogInformation($"Authorization completed successfully for user {validateAuthResult.Item2!.uId} to access tenant {validateAuthResult.Item2.tenantId}");

        _logger.LogInformation("Setting user context");

        return new Tuple<bool, IRequestContext?>(true, validateAuthResult.Item2);
    }

    // Extract the bearer token from the HTTP header
    private Tuple<bool, string?> GetAuthTokenFromHeader(HttpContext? httpContext)
    {
        _logger.LogInformation("About to get Auth Token");
        if (httpContext == null ||!httpContext.Request.Headers.ContainsKey("Authorization"))
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
    private Tuple<bool, string?> GetTenantFromHeader(HttpContext? httpContext)
    {
        _logger.LogInformation("About to validate tenant Id");
        if (httpContext == null || !httpContext.Request.Headers.ContainsKey("tenantId"))
        {
            _logger.LogInformation("tenantId not included in header");
            return new Tuple<bool, string?>(false, ""); ;
        }

        var tenantId = httpContext.Request.Headers["tenantId"].ToString();
        _logger.LogInformation($"tenantId from header is {tenantId}");

        return new Tuple<bool, string?>(true, tenantId);
    }
}
