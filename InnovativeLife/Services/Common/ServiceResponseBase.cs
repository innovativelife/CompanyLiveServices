using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.Services.Common;

public class ServiceResponseBase
{
    public ServiceResponseBase(ResponseStatus status, string message)
    {
        this.message = message;
        Status = status;
    }

    public ServiceResponseBase(DalResponse.ResponseStatus status, string message)
    {
        this.message = message;
        switch (status){
            case DalResponse.ResponseStatus.Ok:
                this.Status = ResponseStatus.Ok;
                break;
            case DalResponse.ResponseStatus.BusinessError:
                this.Status = ResponseStatus.BusinessError;
                break;
            case DalResponse.ResponseStatus.Exception:
                this.Status = ResponseStatus.Exception;
                break;
            case DalResponse.ResponseStatus.NotFound:
                this.Status = ResponseStatus.NotFound;
                break;
            default:
                throw new ApplicationException("Could not map Dal Status to Service Response Statuse");
        }
    }

    public enum ResponseStatus : int
    {
        Ok,

        BusinessError,

        NotFound,

        Exception,

        BadRequest
    }

    public string message {get; set;} = "";

    public ResponseStatus Status {get; set;} = ResponseStatus.Ok;

    public bool Success { get { return (Status == ResponseStatus.Ok); } }
}
