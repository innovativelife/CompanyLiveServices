using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InnovativeLife.GcpServices.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace InnovativeLife.Security;

public class GoogleIdentityAuthenticationHandler : BaseAuthenticationHandler
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

        _userContext.developmentMode = false;

        // Bypass authentciation if AllowAnonymous is set for end point - eg. GetConfig
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            Logger.LogInformation("Endpoint {EndpointName} allows anonymous access. Skipping authentication.", endpoint.DisplayName);
            return AuthenticateResult.NoResult();
        }

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

        var tenant = GetTenantFromUrl(Request, _logger);
        if (!tenant.Item1)
        {
            _logger.LogWarning("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: tenantId not included in url");
            return AuthenticateResult.Fail($"tenantId not included in url");
        }
        _logger.LogInformation($"GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: Tenant ID from URL is: {tenant}");

        _logger.LogInformation("GoogleIdentityAuthenticationHandler.HandleAuthenticateAsync: About to validate token and tenant");

        var AuthResult = await _identityService.AuthenticateUserAndTenant(authToken.Item2!, tenant.Item2!, _userContext, this.Scheme.Name);

        // Final check to ensure everything is set up as expected
        if (AuthResult.Succeeded)
        {
            finalCheck(authToken.Item2!, tenant.Item2!, _logger, _userContext);
        }

        return AuthResult;
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
}