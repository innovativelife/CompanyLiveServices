using InnovativeLife.DataAccess.Common;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantAddResponse : ServiceResponseBase
{
     public TenantAddResponse(ResponseStatus status, string message) : base(status, message) { }
     public TenantAddResponse(DalResponse.ResponseStatus status, string message) : base(status, message) { }
}