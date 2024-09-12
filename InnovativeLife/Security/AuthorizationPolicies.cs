using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.Security;

public static class AuthorizationPolicies
{
    private const string UidClaim = "UId";
    private const string DisplayNameClaim = "DisplayName";
    private const string ActiveClaim = "Active";
    private const string EmailClaim = "Email";
    private const string PhoneNumberClaim = "PhoneNumber";
    private const string TenantIdClaim = "TenantId";
    private const string TenantNameClaim = "TenantName";
    private const string AdminPrivilegeClaim = "TenantAdmin";
    private const string RootAdminClaim = "RootAdmin";
    private const string DevelopmentModeClaim = "DevelopmentMode";

    public static void GetSuperUserPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.RequireClaim(RootAdminClaim, ["True"]);
    }

    public static void GetAdminPrivilegePolicy(AuthorizationPolicyBuilder policy)
    {
        policy.RequireClaim(AdminPrivilegeClaim, "True");
    }

    public static List<Claim> GetClaims(IUserContext userContext, ILogger logger)
    {
        logger.LogInformation($"About to get authorization claims for uId: {userContext.uId}");

        return new List<Claim>()
        {
            new Claim(UidClaim, userContext.uId),
            // new Claim(DisplayNameClaim, userContext.displayName),
            // new Claim(ActiveClaim, userContext.active.ToString()),
            // new Claim(EmailClaim, userContext.email),
            // new Claim(PhoneNumberClaim, userContext.phoneNumber),
            // new Claim(TenantIdClaim, userContext.tenantId),
            // new Claim(TenantNameClaim, userContext.tenantName),
            new Claim(AdminPrivilegeClaim, (userContext.adminPrivilege || userContext.rootAdmin).ToString()),
            new Claim(RootAdminClaim, userContext.rootAdmin.ToString()),
            new Claim(DevelopmentModeClaim, userContext.developmentMode.ToString())
        };
    }
}