using InnovativeLife.Common;

namespace InnovativeLife.WebApi.Common;

public interface ICloudFunctionHandler
{
    public Task<WebResponse> ExecuteService(RequestContext userContext, string method, string[] parameters, string body);
}