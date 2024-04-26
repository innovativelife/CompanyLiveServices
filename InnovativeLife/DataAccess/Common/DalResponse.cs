namespace InnovativeLife.DataAccess.Common;

public class DalResponse
{


    public DalResponse(ResponseStatus status)
    {
        Status = status;
    }

    public enum ResponseStatus : int
    {
        Ok,

        BusinessError,

        NotFound,

        Exception
    }

    public ResponseStatus Status { get; set; } = ResponseStatus.Ok;
    public bool Success { get { return (Status == ResponseStatus.Ok); } }
}