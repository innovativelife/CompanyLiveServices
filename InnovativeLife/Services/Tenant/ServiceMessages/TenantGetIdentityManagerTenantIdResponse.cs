using InnovativeLife.Services.Common;
using InnovativeLife.Services.Employee.ServiceMessages;

namespace InnovativeLife.Services.Tenant.ServiceMessages;

public class TenantGetIdentityManagerTenantIdResponse : ServiceResponseBase
{
    public TenantGetIdentityManagerTenantIdResponse(ResponseStatus status, string message) : base(status, message) { }

    public string identityManagerTenantIdTenantId { get; set; } = "";
}