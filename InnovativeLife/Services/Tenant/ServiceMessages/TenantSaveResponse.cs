using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantSaveResponse : ServiceResponseBase
{
     public TenantSaveResponse(ResponseStatus status, string message) : base(status, message) { }
     public TenantSaveResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public TenantSaveResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
     public TenantItem? tenant { get; set; }
}