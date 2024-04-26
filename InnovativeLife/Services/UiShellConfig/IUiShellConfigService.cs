using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;

namespace InnovativeLife.Services.UiShellConfig;

public interface IUiShellConfigService
{
     public Task<WebResponse> Read(RequestContext userContext, string configId);

    public Task<WebResponse> Save(RequestContext userContext, string configId, UiShellConfigModel configModel);
}
