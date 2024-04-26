using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.User.ServiceMessages;

public class UserCreateRequest : ServiceRequestBase
{
    public string displayName
    {
        get { return preferredName + " " + lastName; }
    }
    public string email { get; set; } = "";
    public string phoneNumber { get; set; } = "";
    public string initialPassword { get; set; } = "";
    public string firstName { get; set; } = "";
    public string lastName { get; set; } = "";

    private string _preferredName = "";
    public string preferredName
    {
        get { return string.IsNullOrEmpty(_preferredName) ? firstName : _preferredName; }
        set { _preferredName = value; }
    }
}