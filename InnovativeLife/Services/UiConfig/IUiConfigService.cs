using InnovativeLife.Security;
using InnovativeLife.Services.UiConfig.ServiceMessages;

namespace InnovativeLife.Services.UiConfig;

public interface IUiConfigService
{
    public Task<UiConfigReadResponse> Read(IUserContext requestContext, string tenantId);

    public Task<UiConfigSaveResponse> Save(IUserContext requestContext, string tenantId, UiConfigSaveRequest configModel);
}
