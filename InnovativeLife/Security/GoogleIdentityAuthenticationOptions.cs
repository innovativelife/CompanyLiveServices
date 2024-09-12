using Microsoft.AspNetCore.Authentication;
using InnovativeLife.GcpServices.Identity;

namespace InnovativeLife.Security;
public class GoogleIdentityAuthenticationOptions : AuthenticationSchemeOptions
{
    public string DisplayMessage { get; set; } = "Authentication failed";
    public const string DefaultScheme = "GoogleIdentityPlatform";
    public string TokenHeaderName { get; } = "Authorization";
    public string UiDHeader {get;} = "uId";
    public string TentantIdHeader { get; } = "tenantid";
}