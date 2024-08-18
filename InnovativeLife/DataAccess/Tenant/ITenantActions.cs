using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Tenant;

public interface ITenantActions
{
    public Task<Tuple<DalResponse, TenantModel?>> Read(string tenantId);

    public Task<Tuple<DalResponse, List<TenantModel?>>> ReadSet();

    public Task<Tuple<DalResponse, TenantModel?>> ReadByName(string tenantName);

    public Task<DalResponse> Save(TenantModel tenantModel);
}