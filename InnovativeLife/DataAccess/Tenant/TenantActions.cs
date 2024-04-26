using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Tenant;

public class TenantActions : ITenantActions
{
    private ILogger<ITenantActions> _logger;
    public TenantActions(ILogger<ITenantActions> logger)
    {
        _logger = logger;
    }

    public async Task<Tuple<DalResponse, TenantModel?>> Read(string tenantId)
    {
        _logger.LogInformation("Reading tenant {0}", tenantId);
        var db = Utilities.connectToFirestore();
        Query tenantQuery = db.Collection(TenantConstants.TenantCollection).WhereEqualTo(TenantConstants.tenantId, tenantId);
        QuerySnapshot tenantQuerySnapshot = await tenantQuery.GetSnapshotAsync();

        if (tenantQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new TenantModel());
        }

        try
        {
            var value = tenantQuerySnapshot[0].ConvertTo<TenantModel>();
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading tenant by TenantId: {ex.Message}");
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new TenantModel());
        }
    }

    public async Task<Tuple<DalResponse, TenantModel?>> ReadByName(string tenantName)
    {
        _logger.LogInformation("Reading tenant by tenant name {0}", tenantName);
        var db = Utilities.connectToFirestore();
        Query tenantQuery = db.Collection(TenantConstants.TenantCollection).WhereEqualTo(TenantConstants.tenantName, tenantName);
        QuerySnapshot tenantQuerySnapshot = await tenantQuery.GetSnapshotAsync();

        if (tenantQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new TenantModel());
        }

        try
        {
            var value = tenantQuerySnapshot[0].ConvertTo<TenantModel>();
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading tenant by tenant name: {ex.Message}");
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new TenantModel());
        }
    }

    public async Task<DalResponse> Save(TenantModel tenantModel)
    {
        _logger.LogInformation("Saving tenant {0}", tenantModel.tenantId);

        var db = Utilities.connectToFirestore();
        CollectionReference collection = db.Collection(TenantConstants.TenantCollection);
        DocumentReference tenantRef = db.Collection(TenantConstants.TenantCollection).Document(tenantModel.tenantId);

        var result = await tenantRef.SetAsync(tenantModel);

        return new DalResponse(DalResponse.ResponseStatus.Ok);
    }
}