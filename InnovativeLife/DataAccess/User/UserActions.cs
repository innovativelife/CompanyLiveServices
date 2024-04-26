using Google.Cloud.Firestore;
using InnovativeLife.DataAccess.Common;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.DataAccess.User;

public class UserActions : IUserActions
{
    private ILogger<IUserActions> _logger;
    public UserActions(ILogger<IUserActions> logger)
    {
        _logger = logger;
    }

    async Task<Tuple<DalResponse, UserModel?>> IUserActions.ReadByIdentifier(string identifier)
    {
        var db = Utilities.connectToFirestore();
        Query userQuery = db.Collection(UserContants.userCollection).WhereEqualTo(UserContants.identifier, identifier);
        QuerySnapshot userQuerySnapshot = await userQuery.GetSnapshotAsync();

        if (userQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, UserModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
        }

        var value = userQuerySnapshot[0].ConvertTo<UserModel>();

        _logger.LogInformation("User Read by Identifier Complete");

        return new Tuple<DalResponse, UserModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
    }

    async Task<Tuple<DalResponse, UserModel?>> IUserActions.ReadByUID(string userUID)
    {
        var db = Utilities.connectToFirestore();
        Query userQuery = db.Collection(UserContants.userCollection).WhereEqualTo(UserContants.userUID, userUID);
        QuerySnapshot userQuerySnapshot = await userQuery.GetSnapshotAsync();

        if (userQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, UserModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
        }

        var value = userQuerySnapshot[0].ConvertTo<UserModel>();

        _logger.LogInformation("User Read by UID Complete");

        return new Tuple<DalResponse, UserModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
    }

    async Task<Tuple<DalResponse, UserModel?>> IUserActions.Save(string userUID, UserModel userModel)
    {
         _logger.LogInformation("Reading user {0}", userModel.userUID);

        var db = Utilities.connectToFirestore();
        CollectionReference collection = db.Collection(UserContants.userCollection);
        DocumentReference userRef = db.Collection(UserContants.userCollection).Document(userUID);

        var result = await userRef.SetAsync(userModel);

        return new Tuple<DalResponse, UserModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), userModel);
    }
}
