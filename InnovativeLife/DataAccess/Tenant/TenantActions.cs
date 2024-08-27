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
        try
        {
            _logger.LogInformation("TenantActions.Read: Reading tenant {0}", tenantId);
            var db = Utilities.connectToFirestore();
            Query tenantQuery = db.Collection(TenantConstants.TenantCollection).WhereEqualTo(TenantConstants.tenantId, tenantId);
            QuerySnapshot tenantQuerySnapshot = await tenantQuery.GetSnapshotAsync();

            if (tenantQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new TenantModel());
            }


            var value = tenantQuerySnapshot[0].ConvertTo<TenantModel>();
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"TenantActions.Read: Error reading tenant by TenantId: {ex.Message}");
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new TenantModel());
        }
    }

    public async Task<Tuple<DalResponse, List<TenantModel?>>> ReadSet()
    {
        try
        {
            _logger.LogInformation("TenantActions.ReadSet: Reading list of tenants");

            var db = Utilities.connectToFirestore();
            Query tenantQuery = db.Collection(TenantConstants.TenantCollection);
            tenantQuery.Limit(100);
            var snapshot = await tenantQuery.GetSnapshotAsync();

            var tenantListModel = new List<TenantModel?>();
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                var tenant = documentSnapshot.ConvertTo<TenantModel>();
                tenantListModel.Add(tenant);
            }

            return new Tuple<DalResponse, List<TenantModel?>>(new DalResponse(DalResponse.ResponseStatus.Ok), tenantListModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"TenantActions.ReadSet: Error reading tenant by tenant name: {ex.Message}");
            return new Tuple<DalResponse, List<TenantModel?>>(new DalResponse(DalResponse.ResponseStatus.Exception), new List<TenantModel>());
        }
    }

    public async Task<Tuple<DalResponse, TenantModel?>> ReadByName(string tenantName)
    {
        try
        {
            _logger.LogInformation("TenantActions.ReadByName: Reading tenant by tenant name {0}", tenantName);
            var db = Utilities.connectToFirestore();
            Query tenantQuery = db.Collection(TenantConstants.TenantCollection).WhereEqualTo(TenantConstants.tenantName, tenantName);
            QuerySnapshot tenantQuerySnapshot = await tenantQuery.GetSnapshotAsync();

            if (tenantQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new TenantModel());
            }

            var value = tenantQuerySnapshot[0].ConvertTo<TenantModel>();
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"TenantActions.ReadByName: Error reading tenant by tenant name: {ex.Message}");
            return new Tuple<DalResponse, TenantModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new TenantModel());
        }
    }

    public async Task<DalResponse> Save(TenantModel tenantModel)
    {
        try{
        _logger.LogInformation("Saving tenant {0}", tenantModel.tenantId);

        var db = Utilities.connectToFirestore();
        CollectionReference collection = db.Collection(TenantConstants.TenantCollection);
        DocumentReference tenantRef = db.Collection(TenantConstants.TenantCollection).Document(tenantModel.tenantId);

        var result = await tenantRef.SetAsync(tenantModel);

        return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError($"TenantActions.Save: Error reading tenant by tenant name: {ex.Message}");
            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }
}