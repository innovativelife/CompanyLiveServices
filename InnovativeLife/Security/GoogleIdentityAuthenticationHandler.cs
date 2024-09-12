
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InnovativeLife.GcpServices.Identity;
using System.Security.Claims;
using InnovativeLife.Common;

namespace InnovativeLife.Security;

public class GoogleIdentityAuthenticationHandler : AuthenticationHandler<GoogleIdentityAuthenticationOptions>
{
    private readonly ILogger<GoogleIdentityAuthenticationHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    public GoogleIdentityAuthenticationHandler(IOptionsMonitor<GoogleIdentityAuthenticationOptions> options, ILoggerFactory logger, IIdentityService identityService, IUserContext userContext, UrlEncoder encoder) : base(options, logger, encoder)
    {
        _logger = logger.CreateLogger<GoogleIdentityAuthenticationHandler>();
        _identityService = identityService;
        _userContext = userContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        _logger.LogInformation("Executing GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync");

        if (InDevMode())
        {
            return SetUpDevelopmentModeContext();
        }
        else
        {
            return await Authenticate();
        }

        AuthenticateResult SetUpDevelopmentModeContext()
        {

            if (Request == null || !Request.Headers.ContainsKey(Options.TentantIdHeader))
            {
                _logger.LogInformation($"GoogleIdentityAuthenticationHandler.GetTenantFromHeader: {Options.TentantIdHeader} must be included in header in dev mode");
                return AuthenticateResult.Fail($"{Options.TentantIdHeader} must be included in header in dev mode");
            }
            var tenantId = Request.Headers[Options.TentantIdHeader].ToString();

            if (Request == null || !Request.Headers.ContainsKey(Options.UiDHeader))
            {
                _logger.LogInformation($"GoogleIdentityAuthenticationHandler.GetTenantFromHeader: {Options.UiDHeader} must be included in header in dev mode");
                return AuthenticateResult.Fail($"{Options.UiDHeader} must be included in header in dev mode");
            }
            var uId = Request.Headers[Options.UiDHeader].ToString();

            _logger.LogInformation($"GoogleIdentityAuthenticationHandler.GetTenantFromHeader: {Options.TentantIdHeader} from header is {tenantId}");
            _logger.LogInformation($"GoogleIdentityAuthenticationHandler.GetTenantFromHeader: {Options.UiDHeader} from header is {uId}");

            _userContext.SetDevelopmentModeContext(tenantId, uId);

            var claims = AuthorizationPolicies.GetClaims(_userContext, _logger);
            var claimsIdentity = new ClaimsIdentity(claims, this.Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, this.Scheme.Name));
        }

        async Task<AuthenticateResult> Authenticate()
        {
            _userContext.developmentMode = false;

            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
            {
                _logger.LogWarning("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsyncMissing authorization token in header");
                return AuthenticateResult.Fail($"Missing header: {Options.TokenHeaderName}");
            }

            var authToken = GetAuthTokenFromHeader(Request);
            if (!authToken.Item1)
            {
                _logger.LogWarning("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: Invalid format of auth token");
                return AuthenticateResult.Fail($"Invalid Authorisation Token");
            }

            var tenant = GetTenantFromUrl(Request);
            if (!tenant.Item1)
            {
                _logger.LogWarning("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: tenantId not included in url");
                return AuthenticateResult.Fail($"tenantId not included in url");
            }
            _logger.LogInformation($"GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: Tenant ID from URL is: {tenant}");

            _logger.LogInformation("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: About to validate token and tenant");
            return await _identityService.AuthenticateUserAndTenant(authToken.Item2!, tenant.Item2!, _userContext, this.Scheme.Name);
        }
    }

    private bool InDevMode()
    {
        // Determine if executing in development mode
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        var devMode = env != null && env.ToLower() == "development";
        if (devMode)
        {
            _logger.LogWarning("GoogleIdentityAuthenticationHandler.InDevMode: In Dev Mode");
        }
        return devMode;
    }

    // Extract the bearer token from the HTTP header
    private Tuple<bool, string?> GetAuthTokenFromHeader(HttpRequest request)
    {
        var authorization = request.Headers[Options.TokenHeaderName].ToString();
        var tokenComponents = authorization.Split("Bearer ");

        if (tokenComponents.Length == 0)
        {
            _logger.LogWarning("GoogleIdentityAuthenticationHandler.GetAuthTokenFromHeader: Invalid format of bearer token");
            return new Tuple<bool, string?>(false, "");
        }

        var token = tokenComponents[1];
        _logger.LogInformation($"GoogleIdentityAuthenticationHandler.GetAuthTokenFromHeader: Token Length: {token.Length}");

        return new Tuple<bool, string?>(true, token);
    }

    // Extract the tenant id from the URL
    private Tuple<bool, string?> GetTenantFromUrl(HttpRequest request)
    {
        _logger.LogInformation("GoogleIdentityAuthenticationHandler.GetTenantFromHeader: About to validate tenant Id");

        // All URL's are of form: /[EntityName]/[tenantId]/{extra segments as required}  eg. Employees/tenant1 for operations on employees in Tenant 1
        if (request == null || request.Path == null || String.IsNullOrEmpty(request.Path.Value))
        {
            _logger.LogError("Null request or path?");
            return new Tuple<bool, string?>(false, "");
        }

        var urlParts = request.Path.Value.Split("/");

        // If a tenant operation, then tenant is not required.  This operation must be performed by user in Root tenant.
        if (urlParts.Length <= 3 && urlParts[1].ToLower() == "tenants")
        {
            return new Tuple<bool, string?>(true, GcpConstants.RootTenantId);
        }

        // Otherwise, Tenant must be second URL parameter
        if (urlParts.Length < 3)
        {
            _logger.LogError("Invalid URL path");
            return new Tuple<bool, string?>(false, "Invalid URL");
        }

        return new Tuple<bool, string?>(true, urlParts[2]);
    }
}