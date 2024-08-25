using Google.Cloud.Firestore;
using InnovativeLife.WebApi;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.DataAccess.UiShellConfig;

public class UiShellConfigActions : IUiShellConfigActions
{
    private ILogger<UiShellConfigActions> _logger;
    public UiShellConfigActions(ILogger<UiShellConfigActions> logger)
    {
        _logger = logger;
    }

    public async Task<Tuple<WebResponse, UiShellConfigModel?>> Read(string configId)
    {
        var db = Utilities.connectToFirestore();
        Query uiShellConfigQuery = db.Collection(UiShellConfigConstants.uiShellConfigCollectionName).WhereEqualTo(UiShellConfigConstants.configId, configId);
        QuerySnapshot uiShellConfigQuerySnapshot = await uiShellConfigQuery.GetSnapshotAsync();

        if (uiShellConfigQuerySnapshot.Count == 0)
        {
            return new Tuple<WebResponse, UiShellConfigModel?>(StandardResponse.NotFound, null);
        }

        var value = uiShellConfigQuerySnapshot[0].ConvertTo<UiShellConfigModel>();

        _logger.LogInformation("GetUiConfig Read Complete");

        return new Tuple<WebResponse, UiShellConfigModel?>(StandardResponse.Success, value);
    }

    public async Task<WebResponse> Save(string configId, UiShellConfigModel uiShellConfigModel)
    {
        _logger.LogInformation("Reading ui config {0}", uiShellConfigModel.configId);

        var db = Utilities.connectToFirestore();
        CollectionReference collection = db.Collection(UiShellConfigConstants.uiShellConfigCollectionName);
        DocumentReference uiConfigRef = db.Collection(UiShellConfigConstants.uiShellConfigCollectionName).Document(uiShellConfigModel.configId);

        var result = await uiConfigRef.SetAsync(uiShellConfigModel);

        return StandardResponse.SuccessWithBody(result.UpdateTime.ToString());
    }
}