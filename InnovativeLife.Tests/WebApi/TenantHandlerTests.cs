using Moq;
using Microsoft.Extensions.Logging;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Tenant;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Common;
using InnovativeLife.Services.Common;
using System.Text.Json;
using System.Net;

namespace InnovativeLife.Tests.WebApi;

public class RouterTests
{
    [Fact]
    public async void EnsureInvalidActionReturnsError()
    {
        var logger =  Mock.Of<ILogger<ITenantHandler>>();
        var tenantService = new Mock<ITenantService>();
        var requestContext = new RequestContext();
        var addTenantRequest = new TenantAddRequest
        {
            tenantId = "xxx",
            tenantName = "Xxxx Xxxx Xxxx"
        };

        var tenantAddResponse = new TenantAddResponse(ServiceResponseBase.ResponseStatus.Ok, "");

        tenantService.Setup(x => x.Add(requestContext, It.IsAny<TenantAddRequest>()))
            .Returns(Task.FromResult(tenantAddResponse));
        var tenantHandler = new TenantHandler(logger, tenantService.Object);

        var result = await tenantHandler.ExecuteService(requestContext, "GET", new string[] {"Invalid"}, "");
        Assert.False(result.Success);
    }

    [Fact]
    public async void EnsureInvalidPayloadReturnsInvalidRequestStatus()
    {
        var logger =  Mock.Of<ILogger<ITenantHandler>>();
        var tenantService = new Mock<ITenantService>();
        var requestContext = new RequestContext();

        var tenantAddResponse = new TenantAddResponse(ServiceResponseBase.ResponseStatus.Ok, "");

        tenantService.Setup(x => x.Add(requestContext, It.IsAny<TenantAddRequest>()))
            .Returns(Task.FromResult(tenantAddResponse));
        var tenantHandler = new TenantHandler(logger, tenantService.Object);

        var result = await tenantHandler.ExecuteService(requestContext, "GET", new string[] {"Add"}, "Any old crap");
        Assert.False(result.Success);
        Assert.Equal(InnovativeLife.WebApi.Common.WebResponse.StatusTypes.InvalidRequest, result.StatusType);
    }

    [Fact]
    public async void EnsureTenantAddRequestCallsTenantServiceCorectly()
    {
        var logger =  Mock.Of<ILogger<ITenantHandler>>();
        var tenantService = new Mock<ITenantService>();
        var requestContext = new RequestContext();
        var addTenantRequest = new TenantAddRequest
        {
            tenantId = "xxx",
            tenantName = "Xxxx Xxxx Xxxx"
        };

        var tenantAddResponse = new TenantAddResponse(ServiceResponseBase.ResponseStatus.Ok, "");

        TenantAddRequest actualAddRequest = new TenantAddRequest();

        tenantService.Setup(x => x.Add(requestContext, It.IsAny<TenantAddRequest>()))
            .Callback<RequestContext, TenantAddRequest>((ignore, val) => actualAddRequest = val)
            .Returns(Task.FromResult(tenantAddResponse));
        var tenantHandler = new TenantHandler(logger, tenantService.Object);

        var result = await tenantHandler.ExecuteService(requestContext, "GET", new string[] {"Add"}, JsonSerializer.Serialize(addTenantRequest));
        Assert.True(result.Success);
        Assert.Equal("xxx", actualAddRequest.tenantId);
        Assert.Equal("Xxxx Xxxx Xxxx", actualAddRequest.tenantName);
    }
}