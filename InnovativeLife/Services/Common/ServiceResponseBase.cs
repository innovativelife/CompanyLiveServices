using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.Services.Common;

public class ServiceResponseBase
{
    public ServiceResponseBase(ResponseStatus status, string message)
    {
        this.Message = message;
        Status = status;
    }

    public ServiceResponseBase(ResponseStatus status, List<string> messages)
    {
        this.Messages = messages;
        Status = status;
    }

    public ServiceResponseBase(DalResponse.ResponseStatus status, string message)
    {
        this.Message = message;
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

    public string Message {
        get
        {
            var error = Messages.Count > 2 ? "errors" : "error";
            return  Messages.Count == 0 ? "" : Messages[0] + (Messages.Count > 1 ? $" (plus {Messages.Count - 1} other {error})" : "");
        } 
        set
        {
            Messages.Add(value); 
        }
    }

    public List<string> Messages {get; set;} = new List<string>();

    public ResponseStatus Status {get; set;} = ResponseStatus.Ok;

    public bool Success { get { return (Status == ResponseStatus.Ok); } }
}
