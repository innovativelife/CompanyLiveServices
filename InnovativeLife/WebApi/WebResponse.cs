using InnovativeLife.Services.Common;

namespace InnovativeLife.WebApi;

public class WebResponse
{
    public enum StatusTypes
    {
        Success = 200,
        InvalidRequest = 400,
        Unauthorised = 401,
        NotFound = 404,
        InvalidData = 422,
        Error = 500,
    }

    public WebResponse(StatusTypes status, String responseData)
    {
        this.StatusType = status;
        this.ResponseData = responseData;
    }

    public WebResponse(ServiceResponseBase.ResponseStatus status, String responseData)
    {
       switch (status)
       {
        case ServiceResponseBase.ResponseStatus.Ok: 
            StatusType = StatusTypes.Success; 
            break;

         case ServiceResponseBase.ResponseStatus.BadRequest:
            StatusType = StatusTypes.InvalidRequest;
            break;

        case ServiceResponseBase.ResponseStatus.InvalidData:
            StatusType = StatusTypes.InvalidData;
            break;

        case ServiceResponseBase.ResponseStatus.BusinessError:
            StatusType = StatusTypes.Success;
            break;

        case ServiceResponseBase.ResponseStatus.NotFound:
            StatusType = StatusTypes.NotFound;
            break;

        case ServiceResponseBase.ResponseStatus.Exception:
            StatusType = StatusTypes.Error;
            break;

        default: 
            throw new ApplicationException("Could not map Service Response Status to a Web Status");
       }
        ResponseData = responseData;
    }

    public bool Success { get { return (StatusType == StatusTypes.Success); } }
    public StatusTypes StatusType { get; }
    public int StatusCode { get { return (int)StatusType; } }
    public String ResponseData { get; }
}