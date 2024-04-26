using InnovativeLife.Common;

namespace InnovativeLife.Services.Common;

public class ServiceRequestBase
{
    public RequestContext userContext{ get; set; } = new RequestContext();

}