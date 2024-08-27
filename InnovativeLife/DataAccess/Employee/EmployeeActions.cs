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
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByIdentifier: User Read by Identifier Starting - {identifier}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.userUID, identifier);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByIdentifier: User Read by Identifier Complete - {identifier}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.ReadByIdentifier: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Exception), new Employee());
        }
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByUID(string userUID)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByUID: Employee Read by UID Starting = {userUID}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.userUID, userUID);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByUID: Employee Read by UID Complete = {userUID}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeActions.ReadByUID: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), new Employee());
        }
    }

    async Task<DalResponse> IEmployeeActions.Save(string userUID, Employee employeeModel)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.Save: Starting read for employee UID - {employeeModel.userUID}");

            var db = Utilities.connectToFirestore();
            CollectionReference collection = db.Collection(EmployeeContants.employeeCollection);
            DocumentReference employeeRef = db.Collection(EmployeeContants.employeeCollection).Document(userUID);

            var result = await employeeRef.SetAsync(employeeModel);

            _logger.LogInformation($"EmployeeActions.Save: Finished read for employee {employeeModel.userUID}");

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.Save: Exception {ex.Message}");

            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }
}
