using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using InnovativeLife.WebApi.Common;
using InnovativeLife.Common;
using System.Text;

namespace InnovativeLife.Tests.CloudFunctionHandler.Router;

public class RouterTests
{
    [Fact]
    public async void InvalidRouteReturns400()
    {
        var testRouter = new InnovativeLife.CloudFunctionHandler.Router(Mock.Of<ILogger<InnovativeLife.CloudFunctionHandler.Router>>());

        testRouter.RegisterRoute("GET", "Entity", Mock.Of<ICloudFunctionHandler>());

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/WrongEntity";
        httpContext.Request.Method = "GET";

        var result = await testRouter.RouteRequest(httpContext, new Common.RequestContext());
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async void ValidRouteReturns200()
    {
        var testRouter = new InnovativeLife.CloudFunctionHandler.Router(Mock.Of<ILogger<InnovativeLife.CloudFunctionHandler.Router>>());

        var mockRespose = new WebResponse(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.Ok, "Response Data");
        var mockRoute = new Mock<ICloudFunctionHandler>();
        mockRoute.Setup(x => x.ExecuteService(It.IsAny<RequestContext>(), "GET", new string[] { "Param1", "Param2" }, "here lies the body")).Returns(() => Task.FromResult(mockRespose));

        testRouter.RegisterRoute("GET", "Entity", mockRoute.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/Entity/Param1/Param2";
        httpContext.Request.Method = "GET";
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("here lies the body"));

        var result = await testRouter.RouteRequest(httpContext, new Common.RequestContext());
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Response Data", result.ResponseData);
    }
}