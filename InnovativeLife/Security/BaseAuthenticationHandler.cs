using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security;

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