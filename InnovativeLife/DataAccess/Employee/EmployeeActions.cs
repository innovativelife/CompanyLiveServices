using Google.Cloud.Firestore;
using InnovativeLife.DataAccess.Common;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.DataAccess.Employee;

public class EmployeeActions : IEmployeeActions
{
    private ILogger<IEmployeeActions> _logger;
    public EmployeeActions(ILogger<IEmployeeActions> logger)
    {
        _logger = logger;
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByIdentifier(string identifier)
    {
        var db = Utilities.connectToFirestore();
        Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.userUID, identifier);
        QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

        if (employeeQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
        }

        var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

        _logger.LogInformation("User Read by Identifier Complete");

        return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByUID(string userUID)
    {
        var db = Utilities.connectToFirestore();
        Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.userUID, userUID);
        QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

        if (employeeQuerySnapshot.Count == 0)
        {
            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
        }

        var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

        _logger.LogInformation("Employee Read by UID Complete");

        return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.Save(string userUID, Employee employeeModel)
    {
         _logger.LogInformation("Reading employee {0}", employeeModel.userUID);

        var db = Utilities.connectToFirestore();
        CollectionReference collection = db.Collection(EmployeeContants.employeeCollection);
        DocumentReference employeeRef = db.Collection(EmployeeContants.employeeCollection).Document(userUID);

        var result = await employeeRef.SetAsync(employeeModel);

        return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), employeeModel);
    }
}
