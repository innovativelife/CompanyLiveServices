using Google.Cloud.Firestore;
using InnovativeLife.DataAccess.Common;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.DataAccess.UiConfig;

public class UiConfigActions : IUiConfigActions
{
    private ILogger<UiConfigActions> _logger;
    public UiConfigActions(ILogger<UiConfigActions> logger)
    {
        _logger = logger;
    }

    public async Task<Tuple<DalResponse, UiConfigModel?>> Read(string tenantId)
    {
        try
        {
            var db = Utilities.connectToFirestore();
            Query uiConfigQuery = db.Collection(UiConfigConstants.uiConfigCollectionName).WhereEqualTo(UiConfigConstants.tenantId, tenantId);
            QuerySnapshot uiConfigQuerySnapshot = await uiConfigQuery.GetSnapshotAsync();

            if (uiConfigQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, UiConfigModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new UiConfigModel());
            }

            var value = uiConfigQuerySnapshot[0].ConvertTo<UiConfigModel>();

            _logger.LogInformation("UiConfigActions.Read: GetUiConfig Read Complete");

            return new Tuple<DalResponse, UiConfigModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"UiConfigActions.Read: Exception {ex.Message}");
            return new Tuple<DalResponse, UiConfigModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new UiConfigModel());
        }
    }

    public async Task<DalResponse> Save(string tenantId, UiConfigModel uiConfigModel)
    {
        try
        {
            _logger.LogInformation("UiConfigActions.Save: Saving ui tenant {0}", tenantId);

            var db = Utilities.connectToFirestore();
            CollectionReference collection = db.Collection(UiConfigConstants.uiConfigCollectionName);
            DocumentReference uiConfigRef = db.Collection(UiConfigConstants.uiConfigCollectionName).Document(uiConfigModel.tenantId);

            var result = await uiConfigRef.SetAsync(uiConfigModel);

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError($"UiConfigActions.Save: Exception - {ex.Message}");
            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }
}