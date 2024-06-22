using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;

namespace InnovativeLife.CloudFunctionHandler;

[FunctionsStartup(typeof(Startup))]
public class RequestHandler : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Notfound");
    }
}