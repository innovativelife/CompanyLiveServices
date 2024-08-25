using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace InnovativeLife.Security;
public interface IUserContext
{
    public string uId { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public bool disabled { get; }
    public string email { get; set; }
    public string phoneNumber { get; set; }
    public string tenantId { get; set; }
    public string tenantName { get; set; }
    public bool tenantAdmin { get; set; }
    public bool rootAdmin { get; set; }
    public bool developmentMode { get; set; }
    public void SetDevelopmentModeContext();
}

public class UserContext : IUserContext
{
    public string uId { get; set; } = "";
    public string displayName { get; set; } = "";
    public bool active { get; set; } = false;
    public bool disabled { get { return !active; } }
    public string email { get; set; } = "";
    public string phoneNumber { get; set; } = "";
    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public bool tenantAdmin { get; set; } = false;
    public bool rootAdmin { get; set; } = false;
    public bool developmentMode { get; set; } = false;

    public void SetDevelopmentModeContext()
    {
        uId = "LocalDevUid";
        displayName = "Local Dev User";
        active = true;
        email = "localdev@testing123.com";
        phoneNumber = "0428283192";
        tenantId = "LocalDevTenantId";
        tenantName = "Local Dev Tenant";
        developmentMode = true;
    }
}
