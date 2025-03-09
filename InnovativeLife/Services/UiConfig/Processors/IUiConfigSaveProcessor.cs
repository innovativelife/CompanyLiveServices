using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.UiConfig.Processors;

public interface IUiConfigSaveProcessor
{
    public Task<UiConfigSaveResponse> Save(IUserContext requestContext, string tenantId, UiConfigSaveRequest saveRequest);
}