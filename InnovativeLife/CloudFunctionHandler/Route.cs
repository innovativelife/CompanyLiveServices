using InnovativeLife.WebApi.Common;

namespace InnovativeLife.CloudFunctionHandler;

public class Route{

    private string _method;
    private string _entity;
    private  ICloudFunctionHandler _service;
    
    public Route(string method, string entity, ICloudFunctionHandler service)
    {
        this._method = method;
        this._entity = entity;
        this._service = service;
    }


    public string Method { get => _method;}
    public string Entity { get => _entity;}
    public ICloudFunctionHandler Service { get => _service;}

    public static string GetRouteKey(string method, string entity) {
        return $"{method}.{entity}";
    }

    public override string ToString()
    {
        return GetRouteKey(_method, _entity);
    }
}