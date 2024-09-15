using Microsoft.AspNetCore.Http;
using InnovativeLife.DataAccess.Common;
using System.ComponentModel.Design;

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
        switch (status)
        {
            case DalResponse.ResponseStatus.Ok:
                this.Status = ResponseStatus.Ok;
                break;
            case DalResponse.ResponseStatus.Added:
                this.Status = ResponseStatus.Added;
                break;
            case DalResponse.ResponseStatus.Duplicate:
                this.Status = ResponseStatus.Duplicate;
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

        Added,

        Duplicate,

        InvalidData,

        BusinessError,

        NotFound,

        Exception,

        BadRequest
    }

    public string Message
    {
        get
        {
            var error = Messages.Count > 2 ? "errors" : "error";
            return Messages.Count == 0 ? "" : Messages[0] + (Messages.Count > 1 ? $" (and {Messages.Count - 1} other {error})" : "");
        }
        set
        {
            Messages.Add(value);
        }
    }

    public List<string> Messages { get; set; } = new List<string>();

    public ResponseStatus Status { get; set; } = ResponseStatus.Ok;

    public bool Success { get { return (Status == ResponseStatus.Ok || Status == ResponseStatus.Added); } }

    public IResult GetAspNetResult()
    {
        switch (this.Status)
        {
            case ResponseStatus.Ok:
                return Results.Ok(this);
            case ResponseStatus.Added:
                return Results.Created("", this);
            case ResponseStatus.Duplicate:
                return Results.Conflict(this);
            case ResponseStatus.InvalidData:
                return Results.UnprocessableEntity(this);
        }

        return Results.NotFound(this);
    }
}
