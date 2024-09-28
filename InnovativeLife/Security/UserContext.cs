using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace InnovativeLife.Security;
public interface IUserContext
{
    public string uId { get; set; }
    public string preferredName { get; set; }
    public bool active { get; set; }
    public bool disabled { get; }
    public string email { get; set; }
    public string phoneNumber { get; set; }
    public string tenantId { get; set; }
    public string identityManagerTenantId { get; set; }
    public string customerName { get; set; }
    public bool adminPrivilege { get; set; }
    public bool rootAdmin { get; set; }
    public bool developmentMode { get; set; }
    public void SetDevelopmentModeContext(string uid, string tenantId);
}

public class UserContext : IUserContext
{
    public string uId { get; set; } = "";
    public string preferredName { get; set; } = "";
    public bool active { get; set; } = false;
    public bool disabled { get { return !active; } }
    public string email { get; set; } = "";
    public string phoneNumber { get; set; } = "";
    public string tenantId { get; set; } = "";
    public string identityManagerTenantId { get; set; } = "";
    public string customerName { get; set; } = "";
    public bool adminPrivilege { get; set; } = false;
    public bool rootAdmin { get; set; } = false;
    public bool developmentMode { get; set; } = false;

    public void SetDevelopmentModeContext(string requestTenantId, string requestUid)
    {
        this.developmentMode = true;

        this.rootAdmin = requestTenantId == "Root";
        this.uId = requestUid;
        this.tenantId = requestTenantId;
        this.identityManagerTenantId = Guid.NewGuid().ToString();

        this.preferredName = "Local Dev User";
        this.email = "localdev@testing123.com";
        this.phoneNumber = "0428283192";
        this.active = true;
        this.adminPrivilege = true;
    }
}
