using InnovativeLife.DataAccess.UiShellConfig;
using InnovativeLife.Common;
using InnovativeLife.WebApi.Common;

namespace InnovativeLife.Services.UiShellConfig;

public interface IUiShellConfigService
{
     public Task<WebResponse> Read(RequestContext requestContext, string configId);

    public Task<WebResponse> Save(RequestContext requestContext, string configId, UiShellConfigModel configModel);
}
