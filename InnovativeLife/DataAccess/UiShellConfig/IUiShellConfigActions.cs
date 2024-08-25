using InnovativeLife.WebApi;

namespace InnovativeLife.DataAccess.UiShellConfig;

public interface IUiShellConfigActions
{
    public Task<Tuple<WebResponse, UiShellConfigModel?>> Read(string configId);
    public Task<WebResponse> Save(string configId, UiShellConfigModel uiShellConfigModel);
}