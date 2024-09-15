using Moq;
using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.Services.Tenant.ServiceMessages;
using InnovativeLife.Services.Tenant.Processors;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.DataAccess.Common;
using InnovativeLife.GcpServices.Identity;
using InnovativeLife.Services.Employee;

namespace InnovativeLife.Tests.Services.Tenant.Processors;

public class TenantAddProcessorTests
{
    [Fact]
    public async void EnsureAddCanOnlyBePerformedByRootUser()
    {
        var logger = Mock.Of<ILogger<TenantAddProcessor>>();
        var tenantActions = Mock.Of<ITenantActions>();
        var identityService = Mock.Of<IIdentityService>();
        var employeeService = Mock.Of<IEmployeeService>();
        var tenantAddProcessor = new TenantAddProcessor(logger, tenantActions, identityService, employeeService);

        var requestContext = new UserContext();
        requestContext.rootAdmin = false;
        var addRequest = new TenantAddRequest();
        var result = await tenantAddProcessor.Add(requestContext, addRequest);

        Assert.Equal(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.Exception, result.Status);
        Assert.Equal("Unauthorised Add", result.Message);
    }

    [Fact]
    public async void EnsureTenantIdIsNotEmpty()
    {
        var logger = Mock.Of<ILogger<ITenantAddProcessor>>();
        var dalResponse = new DalResponse(DalResponse.ResponseStatus.Ok);
        var tenantModel = new TenantModel();
        var readResponse = new Tuple<DalResponse, TenantModel>(dalResponse, tenantModel);
        var tenantActions = Mock.Of<ITenantActions>();
        var identityService = Mock.Of<IIdentityService>();
        var employeeService = Mock.Of<IEmployeeService>();

        var tenantAddProcessor = new TenantAddProcessor(logger, tenantActions, identityService, employeeService);

        var requestContext = new UserContext();
        requestContext.rootAdmin = true;
        var addRequest = new TenantAddRequest();
        var result = await tenantAddProcessor.Add(requestContext, addRequest);

        Assert.Equal(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, result.Status);
        Assert.Equal("Tenant ID cannot be left blank", result.Message);
    }

    [Fact]
    public async void EnsureTenantNameIsNotEmpty()
    {
        var logger = Mock.Of<ILogger<ITenantAddProcessor>>();
        var dalResponse = new DalResponse(DalResponse.ResponseStatus.Ok);
        var tenantModel = new TenantModel();
        var readResponse = new Tuple<DalResponse, TenantModel>(dalResponse, tenantModel);
        var tenantActions = Mock.Of<ITenantActions>();
        var identityService = Mock.Of<IIdentityService>();
        var employeeService = Mock.Of<IEmployeeService>();

        var tenantAddProcessor = new TenantAddProcessor(logger, tenantActions, identityService, employeeService);

        var requestContext = new UserContext();
        requestContext.rootAdmin = true;
        var addRequest = new TenantAddRequest();
        addRequest.tenantId = "xxx";
        var result = await tenantAddProcessor.Add(requestContext, addRequest);

        Assert.Equal(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, result.Status);
        Assert.Equal("Tenant Name cannot be left blank", result.Message);
    }

    [Fact]
    public async void EnsureTenantIdIsUnique()
    {
        var logger = Mock.Of<ILogger<TenantAddProcessor>>();

        // Set up first read - By Id return not found
        var dalReadByIdResponse = new DalResponse(DalResponse.ResponseStatus.Ok);
        var readByIdTenantModel = new TenantModel();
        var readByIdResponse = new Tuple<DalResponse, TenantModel>(dalReadByIdResponse, readByIdTenantModel);

        var tenantActions = new Mock<InnovativeLife.DataAccess.Tenant.ITenantActions>();  
        tenantActions.Setup(x => x.Read("yyy")).Returns(Task.FromResult(readByIdResponse));

        var identityService = Mock.Of<InnovativeLife.GcpServices.Identity.IIdentityService>();
        var employeeService = Mock.Of<IEmployeeService>();

        var tenantAddProcessor = new InnovativeLife.Services.Tenant.Processors.TenantAddProcessor(logger, tenantActions.Object, identityService, employeeService);

        var requestContext = new UserContext();
        requestContext.rootAdmin = true;
        var addRequest = new TenantAddRequest();

        // Different Id
        addRequest.tenantId = "yyy";
        // Duplicate name
        addRequest.tenantName = "xxx";
        var result = await tenantAddProcessor.Add(requestContext, addRequest);

        Assert.Equal(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, result.Status);
        Assert.Equal("Tenant with this ID already exists", result.Message);
    }

    [Fact]
    public async void EnsureTenantNameIsUnique()
    {
        var logger = Mock.Of<ILogger<InnovativeLife.Services.Tenant.Processors.TenantAddProcessor>>();

        // Set up first read - By Id return not found
        var dalReadByIdResponse = new DalResponse(DalResponse.ResponseStatus.NotFound);
        var readByIdTenantModel = new TenantModel();
        var readByIdResponse = new Tuple<DalResponse, TenantModel>(dalReadByIdResponse, readByIdTenantModel);

        // Set up second read - By name found
        var dalReadByNameResponse = new DalResponse(DalResponse.ResponseStatus.Ok);
        var readByNameTenantModel = new TenantModel();
        var readByNameResponse = new Tuple<DalResponse, TenantModel>(dalReadByNameResponse, readByNameTenantModel);

        var tenantActions = new Mock<InnovativeLife.DataAccess.Tenant.ITenantActions>();  
        tenantActions.Setup(x => x.Read("yyy")).Returns(Task.FromResult(readByIdResponse));
        tenantActions.Setup(x => x.ReadByName("xxx")).Returns(Task.FromResult(readByNameResponse));

        var identityService = Mock.Of<InnovativeLife.GcpServices.Identity.IIdentityService>();
        var employeeService = Mock.Of<IEmployeeService>();

        var tenantAddProcessor = new InnovativeLife.Services.Tenant.Processors.TenantAddProcessor(logger, tenantActions.Object, identityService, employeeService);

        var requestContext = new UserContext();
        requestContext.rootAdmin = true;
        var addRequest = new TenantAddRequest();

        // Different Id
        addRequest.tenantId = "yyy";
        // Duplicate name
        addRequest.tenantName = "xxx";
        var result = await tenantAddProcessor.Add(requestContext, addRequest);

        Assert.Equal(InnovativeLife.Services.Common.ServiceResponseBase.ResponseStatus.BusinessError, result.Status);
        Assert.Equal("Tenant with this name already exists", result.Message);
    }

    // EnsureIdentityServiceCalledCorrectly

    // TestTenantActionsCalledCorrectly

}