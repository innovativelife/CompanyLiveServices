
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InnovativeLife.GcpServices.Identity;
using System.Security.Claims;
using Microsoft.VisualBasic;

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

        _logger.LogInformation("Constructing GoogleIdentityAuthenticationHandler");
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        _logger.LogInformation("Executing HandleAuthenticateAsync");

        _userContext.developmentMode = InDevMode();

        if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
        {
            _logger.LogInformation("Missing authorization token in header");
            return AuthenticateResult.Fail($"Missing header: {Options.TokenHeaderName}");
        }

        var authToken = GetAuthTokenFromHeader(Request);
        if (!authToken.Item1)
        {
            _logger.LogWarning("Invalid format of auth token");
            return AuthenticateResult.Fail($"Invalid Authorisation Token");
        }

        var tenant = GetTenantFromHeader(Request);
        if (!tenant.Item1)
        {
            _logger.LogWarning("tenantId not included in header");
            return AuthenticateResult.Fail($"tenantId not included in header");
        }
        _logger.LogWarning($"Tenant ID from header is: {tenant}");

        _logger.LogWarning("About to validate token and tenant");
        return await _identityService.AuthenticateUserAndTenant(authToken.Item2!, tenant.Item2!, _userContext, this.Scheme.Name);
    }

    private bool InDevMode()
    {
        // Determine if executing in development mode
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return env != null && env.ToLower() == "development";
    }

    // Extract the bearer token from the HTTP header
    private Tuple<bool, string?> GetAuthTokenFromHeader(HttpRequest request)
    {
        var authorization = request.Headers[Options.TokenHeaderName].ToString();
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
    private Tuple<bool, string?> GetTenantFromHeader(HttpRequest request)
    {
        _logger.LogInformation("About to validate tenant Id");
        if (request == null || !request.Headers.ContainsKey(Options.TentantIdHeader))
        {
            _logger.LogInformation($"{Options.TentantIdHeader} not included in header");
            return new Tuple<bool, string?>(false, ""); ;
        }

        var tenantId = request.Headers[Options.TentantIdHeader].ToString();
        _logger.LogInformation($"{Options.TentantIdHeader} from header is {tenantId}");

        return new Tuple<bool, string?>(true, tenantId);
    }
}