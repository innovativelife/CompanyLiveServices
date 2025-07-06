
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace InnovativeLife.Security;

public class LocalDevAuthenticationHandler : BaseAuthenticationHandler
{
    internal readonly ILogger<GoogleIdentityAuthenticationHandler> _logger;
    internal readonly IUserContext _userContext;
    public LocalDevAuthenticationHandler(IOptionsMonitor<GoogleIdentityAuthenticationOptions> options, ILoggerFactory logger, IUserContext userContext, UrlEncoder encoder) : base(options, logger, encoder)
    {
        _logger = logger.CreateLogger<GoogleIdentityAuthenticationHandler>();
        _userContext = userContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        _logger.LogInformation("Executing LocalDevelopmentAuthenticationHandler.HandleAuthenticateAsync");

        // Bypass authentciation if AllowAnonymous is set for end point - eg. GetConfig
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            Logger.LogInformation("Endpoint {EndpointName} allows anonymous access. Skipping authentication.", endpoint.DisplayName);
            return AuthenticateResult.NoResult();
        }

        if (Request == null || !Request.Headers.ContainsKey(Options.TentantIdHeader))
        {
            _logger.LogInformation($"LocalDevelopmentAuthenticationHandler.GetTenantFromHeader: {Options.TentantIdHeader} must be included in header in dev mode");
            return AuthenticateResult.Fail($"{Options.TentantIdHeader} must be included in header in dev mode");
        }
        var tenantId = Request.Headers[Options.TentantIdHeader].ToString();

        if (Request == null || !Request.Headers.ContainsKey(Options.UiDHeader))
        {
            _logger.LogInformation($"LocalDevelopmentAuthenticationHandler.GetTenantFromHeader: {Options.UiDHeader} must be included in header in dev mode");
            return AuthenticateResult.Fail($"{Options.UiDHeader} must be included in header in dev mode");
        }
        var uId = Request.Headers[Options.UiDHeader].ToString();

        _logger.LogInformation($"LocalDevelopmentAuthenticationHandler.GetTenantFromHeader: {Options.TentantIdHeader} from header is {tenantId}");
        _logger.LogInformation($"LocalDevelopmentAuthenticationHandler.GetTenantFromHeader: {Options.UiDHeader} from header is {uId}");

        _userContext.SetDevelopmentModeContext(tenantId, uId);

        var claims = AuthorizationPolicies.GetClaims(_userContext, _logger);
        var claimsIdentity = new ClaimsIdentity(claims, this.Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Final check to ensure everything is set up as expected
        finalCheck(Guid.NewGuid().ToString(), tenantId, _logger, _userContext);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, this.Scheme.Name));
    }

}