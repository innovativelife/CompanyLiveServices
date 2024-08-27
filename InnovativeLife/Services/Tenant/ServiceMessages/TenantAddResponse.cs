using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantAddResponse : ServiceResponseBase
{
     public TenantAddResponse(ResponseStatus status, string message) : base(status, message) { }
     public TenantAddResponse(ResponseStatus status, List<string> messages) : base(status, messages) { }
     public TenantAddResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
     public TenantItem tenant { get; set; } = new TenantItem();
}