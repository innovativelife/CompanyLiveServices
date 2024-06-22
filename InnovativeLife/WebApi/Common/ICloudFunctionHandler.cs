using InnovativeLife.Common;

namespace InnovativeLife.WebApi.Common;

public interface ICloudFunctionHandler
{
    public Task<WebResponse> ExecuteService(RequestContext requestContext, string method, string[] parameters, string body);
}