using InnovativeLife.Services.UiConfig.ServiceMessages;
using InnovativeLife.Security;

namespace InnovativeLife.Services.UiConfig.Processors;

public interface IUiConfigReadProcessor
{
    public Task<UiConfigReadResponse> ReadSingleton(IUserContext requestContext, string tenantId);
}