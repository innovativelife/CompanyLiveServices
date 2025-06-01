using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InnovativeLife.GcpServices.Identity;
using System.Security.Claims;
using InnovativeLife.Common;

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

    // Extract the tenant id from the URL
    private Tuple<bool, string?> GetTenantFromUrl(HttpRequest request)
    {
        _logger.LogInformation("GoogleIdentityAuthenticationHandler.GetTenantFromHeader: About to validate tenant Id");

        // All URL's are of form: /api/v1/Tenants/[tenantId]/[EntityName]/{extra segments as required}  eg. /api/v1/Tenants/tenant1/Employees for operations on employees in Tenant 1
        if (request == null || request.Path == null || String.IsNullOrEmpty(request.Path.Value))
        {
            _logger.LogError("Null request or path?");
            return new Tuple<bool, string?>(false, "");
        }

        var urlParts = request.Path.Value.Split("/");

        // If a admin operation, then tenant is not required.  This operation must be performed by user in Root tenant.
        if (urlParts.Length <= 4 && urlParts[3].ToLower() == "admin")
        {
            return new Tuple<bool, string?>(true, GcpConstants.RootTenantId);
        }

        // Otherwise, Tenant must be second URL parameter
        if (urlParts.Length < 5)
        {
            _logger.LogError("Invalid URL path");
            return new Tuple<bool, string?>(false, "Invalid URL");
        }

        var tenantId = urlParts[4];
        _logger.LogInformation($"Tenant found in URL: {tenantId}");
        return new Tuple<bool, string?>(true, tenantId);
    }
}