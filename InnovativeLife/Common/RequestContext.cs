namespace InnovativeLife.Common;

public interface IRequestContext
{
    public string uId { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public bool disabled { get; }
    public string email { get; set; }
    public string phoneNumber { get;  set; } 
    public string tenantId { get; set; }
    public string tenantName { get; set; }
    public bool tenantAdminPriviledge { get; set; }
    public bool rootPriviledge { get; set; }
    public bool developmentMode { get; set; }
    public void SetDevelopmentModeContext();
}

public class RequestContext : IRequestContext
{
    public string uId { get; set; } = "";
    public string displayName { get; set; } = "";
    public bool active { get; set; }
    public bool disabled { get { return !active; } }
    public string email { get; set; } = "";
    public string phoneNumber { get;  set; } = "";
    public string tenantId { get; set; } = "";
    public string tenantName { get; set; } = "";
    public bool tenantAdminPriviledge { get; set; } = false;
    public bool rootPriviledge { get; set; } = false;
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