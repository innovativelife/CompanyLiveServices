using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security;
using Microsoft.AspNetCore.Http;
using InnovativeLife.Common;

namespace InnovativeLife.Security;

public class BaseAuthenticationHandler : AuthenticationHandler<GoogleIdentityAuthenticationOptions>
{
    public BaseAuthenticationHandler(IOptionsMonitor<GoogleIdentityAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

         // Extract the tenant id from the URL
    internal Tuple<bool, string> GetTenantFromUrl(HttpRequest request, ILogger logger)
    {
        logger.LogInformation("GoogleIdentityAuthenticationHandler.GetTenantFromHeader: About to validate tenant Id");

        // All URL's are of form: /api/v1/Tenants/[tenantId]/[EntityName]/{extra segments as required}  eg. /api/v1/Tenants/tenant1/Employees for operations on employees in Tenant 1
        if (request == null || request.Path == null || String.IsNullOrEmpty(request.Path.Value))
        {
            logger.LogError("Null request or path?");
            return new Tuple<bool, string>(false, "");
        }

        var urlParts = request.Path.Value.Split("/");

        // If a admin operation, url is: /api/v1/admin
        // This operation must be performed by user in Root tenant (checked later).
        if (urlParts.Length >= 4 && urlParts[3].ToLower() == Constants.AdminUrlParameterName)
        {
            logger.LogInformation("Admin function - must be root tenant");
            return new Tuple<bool, string>(true, GcpConstants.RootTenantId);
        }

        // Otherwise, Tenant must be 4th URL parameter
        // ie. /api/v1/tenants/[tenantId]
        if (urlParts.Length < 5)
        {
            logger.LogError("Invalid URL path - Not admin function Url not long enough");
            return new Tuple<bool, string>(false, "Invalid URL");
        }

        if (urlParts[3].ToLower() != Constants.TenantsUrlParameterName)
        {
            logger.LogError($"Invalid URL path - Not admin function and '{Constants.TenantsUrlParameterName}' url parameter missing");
            return new Tuple<bool, string>(false, "Invalid URL");
        }

        var tenantId = urlParts[4];
        logger.LogInformation($"Tenant found in URL: {tenantId}");
        return new Tuple<bool, string>(true, tenantId);
    }

    // A series of tests to ensure everything is as expected - effectively a regression test of code
    internal bool finalCheck(string authToken, string tenantId, ILogger logger, IUserContext userContext)
    {
        logger.LogInformation("Performing final Auth Checks");

        if (string.IsNullOrEmpty(authToken))
        {
            logger.LogCritical("FinalCheck failed - authToken is null or empty");
            throw new SecurityException("FinalCheck failed - authToken is null or empty");
        }

        if (string.IsNullOrEmpty(tenantId))
        {
            logger.LogCritical("FinalCheck failed - tenantId is null or empty");
            throw new SecurityException("FinalCheck failed - tenantId is null or empty");
        }

        if (userContext is null)
        {
            logger.LogCritical("FinalCheck failed - user context is null");
            throw new SecurityException("FinalCheck failed - user context is null");
        }

        if (string.IsNullOrEmpty(userContext.tenantId))
        {
            logger.LogCritical("FinalCheck failed - user context tenant Id is null or empty");
            throw new SecurityException("FinalCheck failed - user context tenant Id is null or empty");
        }

        if (!string.Equals(tenantId, userContext.tenantId))
        {
            logger.LogCritical($"FinalCheck failed - tenant Id not consistent: {tenantId} vs userContext: {userContext.tenantId}");
            throw new SecurityException($"FinalCheck failed - tenant Id not consistent: {tenantId} vs userContext: {userContext.tenantId}");
        }

        if (string.IsNullOrEmpty(userContext.identityManagerTenantId))
        {
            logger.LogCritical("FinalCheck failed - user context identityManagerTenantId is null or empty");
            throw new SecurityException("FinalCheck failed - user context identityManagerTenantId is null or empty");
        }

        if (string.IsNullOrEmpty(userContext.uId))
        {
            logger.LogCritical("FinalCheck failed - user context uId is null or empty");
            throw new SecurityException("FinalCheck failed - user context uId is null or empty");
        }

        if (!userContext.active)
        {
            logger.LogCritical("FinalCheck failed - User is not active");
            throw new SecurityException("FinalCheck failed - User is not active");
        }

        if (userContext.disabled)
        {
            logger.LogCritical("FinalCheck failed - User is disabled");
            throw new SecurityException("FinalCheck failed - User is disabled");
        }

        return true;
    }
    
}