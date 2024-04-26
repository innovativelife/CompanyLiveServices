using Microsoft.AspNetCore.Http;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Common;

namespace InnovativeLife.CloudFunctionHandler;

public interface IRouter
{
    public void RegisterRoute(string method, string entity, ICloudFunctionHandler service);
    
    public Task<WebResponse> RouteRequest(HttpContext context, RequestContext userContext);
}