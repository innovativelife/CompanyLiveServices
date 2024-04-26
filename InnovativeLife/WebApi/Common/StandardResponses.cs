namespace InnovativeLife.WebApi.Common;

public static class StandardResponse
{
    public static WebResponse NotFound
    {
        get { return new WebResponse(WebResponse.StatusTypes.NotFound, "Not Found"); }
    }

    public static WebResponse InvalidRequest
    {
        get { return new WebResponse(WebResponse.StatusTypes.InvalidRequest, "Invalid Request"); }
    }

    public static WebResponse InvalidRequestWithMessage(string errorMessage)
    {
        return new WebResponse(WebResponse.StatusTypes.InvalidRequest, errorMessage);
    }

    public static WebResponse Error
    {
        get { return new WebResponse(WebResponse.StatusTypes.Error, "Error"); }
    }

    public static WebResponse ErrorWithMessage(string errorMessage)
    {
        return new WebResponse(WebResponse.StatusTypes.Error, errorMessage); 
    }

    public static WebResponse Unauthorised
    {
        get { return new WebResponse(WebResponse.StatusTypes.Unauthorised, "Unauthorised"); }
    }

    public static WebResponse Success
    {
        get { return new WebResponse(WebResponse.StatusTypes.Success, "Success"); }
    }

    public static WebResponse SuccessWithBody(string body)
    {
        return new WebResponse(WebResponse.StatusTypes.Success, body);
    }
}