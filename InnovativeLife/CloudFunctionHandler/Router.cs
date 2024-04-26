using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Buffers;
using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;

namespace InnovativeLife.CloudFunctionHandler;

public class Router : IRouter
{
    private Dictionary<string, Route> _register = new Dictionary<string, Route>();

    private ILogger<Router> _logger;

    public Router(ILogger<Router> logger)
    {
        _logger = logger;
    }

    public void RegisterRoute(string method, string entity, ICloudFunctionHandler service)
    {
        if (_register.ContainsKey(Route.GetRouteKey(method, entity)))
        {
            _logger.LogInformation($"Router for entity {entity} already registered");
            return;
        }

        _logger.LogInformation($"Router for entity {entity} registered");

        var route = new Route(method, entity, service);
        _register.Add(route.ToString(), route);
    }

    public async Task<WebResponse> RouteRequest(HttpContext context, RequestContext userContext)
    {
        try
        {

            _logger.LogInformation("Routing request");
            var entityAndParams = GetEntityAndParams(context.Request.Path);

            if (entityAndParams.Length == 0 || !ValidateRequest(context, entityAndParams[0]))
            {
                return StandardResponse.InvalidRequest;
            }

            var routeKey = Route.GetRouteKey(context.Request.Method, entityAndParams[0]);
            if (_register.ContainsKey(routeKey))
            {
                _logger.LogInformation($"Routing request for {_register[routeKey].Entity} / {_register[routeKey].Method}");
                var parameters = (entityAndParams.Length == 0) ? new string[0] : entityAndParams[1..entityAndParams.Length];

                var body = (context.Request == null || context.Request.Body == null) ? "" : await GetStringFromRequest(context.Request.Body); ;
                return await _register[routeKey].Service.ExecuteService(userContext, context.Request.Method, parameters, body);
            }

            return StandardResponse.InvalidRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message}: \n{ex.StackTrace}");
            return StandardResponse.Error;
        }
    }

    private async Task<string> GetStringFromRequest(Stream requestBody)
    {
        // Build up the request body in a string builder.
        StringBuilder builder = new StringBuilder();

        // Rent a shared buffer to write the request body into.
        byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);

        while (true)
        {
            var bytesRemaining = await requestBody.ReadAsync(buffer, offset: 0, buffer.Length);
            if (bytesRemaining == 0)
            {
                break;
            }

            // Append the encoded string into the string builder.
            var encodedString = Encoding.UTF8.GetString(buffer, 0, bytesRemaining);
            builder.Append(encodedString);
        }

        ArrayPool<byte>.Shared.Return(buffer);

        return builder.ToString();
    }

    private string[] GetEntityAndParams(string path)
    {
        var components = path.Split("/");
        if (components.Length < 2)
        {
            return new string[0];
        }

        return components[1..components.Length];
    }

    private bool ValidateRequest(HttpContext context, string entity)
    {
        if (context.Request != null)
        {
            var routeKey = Route.GetRouteKey(context.Request!.Method, entity);
            if (_register.ContainsKey(routeKey))
            {
                return true;
            }
        }

        return false;
    }

}