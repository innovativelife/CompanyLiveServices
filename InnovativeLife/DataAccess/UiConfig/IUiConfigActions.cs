using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.UiConfig;

public interface IUiConfigActions
{
    public Task<Tuple<DalResponse, UiConfigModel?>> Read(string tenantId);
    public Task<DalResponse> Save(string tenantId, UiConfigModel uiConfigModel);
}