using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Security;
using InnovativeLife.WebApi;

namespace InnovativeLife.Services.UiShellConfig;

public interface IUiShellConfigService
{
     public Task<WebResponse> Read(UserContext requestContext, string configId);

    public Task<WebResponse> Save(UserContext requestContext, string configId, UiShellConfigModel configModel);
}
