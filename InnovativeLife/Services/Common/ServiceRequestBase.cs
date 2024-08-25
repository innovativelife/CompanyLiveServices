using InnovativeLife.Security;

namespace InnovativeLife.Services.Common;

public class ServiceRequestBase
{
    public UserContext requestContext{ get; set; } = new UserContext();
}