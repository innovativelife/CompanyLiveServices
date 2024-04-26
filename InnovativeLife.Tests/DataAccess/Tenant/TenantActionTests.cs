using Moq;
using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Tenant;
using InnovativeLife.DataAccess;
using Google.Cloud.Firestore;

namespace InnovativeLife.Tests.DataAccess.Tenant;

public class TenantActionTests
{
    private void UseFireBaseEmulator()
    {
        Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", "localhost:5003");
    }

    private async void ClearTenantCollection()
    {
        var db = Utilities.connectToFirestore();
        QuerySnapshot snapshot = await db.Collection(TenantConstants.tenantId).GetSnapshotAsync();
        IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
        while (documents.Count > 0)
        {
            foreach (DocumentSnapshot document in documents)
            {
                Console.WriteLine("Deleting document {0}", document.Id);
                await document.Reference.DeleteAsync();
            }
        }
    }

    [Fact]
    public async void EnsureReadByTenantOfNonExistingTenantReturnsError()
    {
        UseFireBaseEmulator();
        ClearTenantCollection();

        var logger = Mock.Of<ILogger<ITenantActions>>();
        var tenantActions = new TenantActions(logger);

        var readResponse = await tenantActions.Read("doesNotExist");
        Assert.False(readResponse.Item1.Success);
    }

    [Fact]
    public async void EnsureTenantIsSavedAndReadReturnsTenant()
    {
        UseFireBaseEmulator();
        ClearTenantCollection();

        var logger = Mock.Of<ILogger<ITenantActions>>();
        var tenantActions = new TenantActions(logger);

        var tenantModel = new TenantModel
        {
            tenantId = "xxx",
            tenantName = "Xxxxx Xxxxx Xxxxx",
            identityManagerTenantId = "Guid",
            active = true
        };

        var saveResponse = await tenantActions.Save(tenantModel);
        Assert.True(saveResponse.Success);

        var readResponse = await tenantActions.Read("xxx");
        Assert.True(readResponse.Item1.Success);
        Assert.Equal("xxx", readResponse.Item2.tenantId);
        Assert.Equal("Xxxxx Xxxxx Xxxxx", readResponse.Item2.tenantName);
        Assert.Equal("Guid", readResponse.Item2.identityManagerTenantId);
        Assert.True(readResponse.Item2.active);
    }

    [Fact]
    public async void EnsureTenantIsSavedAndReadByNameReturnsTenant()
    {
        UseFireBaseEmulator();
        ClearTenantCollection();

        var logger = Mock.Of<ILogger<ITenantActions>>();
        var tenantActions = new TenantActions(logger);

        var tenantModel = new TenantModel
        {
            tenantId = "xxx",
            tenantName = "Xxxxx Xxxxx Xxxxx",
            identityManagerTenantId = "Guid",
            active = true
        };

        var saveResponse = await tenantActions.Save(tenantModel);
        Assert.True(saveResponse.Success);

        var readResponse = await tenantActions.ReadByName("Xxxxx Xxxxx Xxxxx");
        Assert.True(readResponse.Item1.Success);
        Assert.Equal("xxx", readResponse.Item2.tenantId);
        Assert.Equal("Xxxxx Xxxxx Xxxxx", readResponse.Item2.tenantName);
        Assert.Equal("Guid", readResponse.Item2.identityManagerTenantId);
        Assert.True(readResponse.Item2.active);
    }
}