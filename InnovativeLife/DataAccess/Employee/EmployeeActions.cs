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

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmployeeUID(string employeeUID)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmployeeUID: User Read by employeeUID Starting - {employeeUID}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.employeeUID, employeeUID);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByEmployeeUID: User Read by EmployeeUID Complete - {employeeUID}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmployeeUID: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Exception), new Employee());
        }
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmployeeNumber(string employeeNumber)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmpoyeeNumber: Employee Read by employeeNumber Starting = {employeeNumber}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.employeeNumber, employeeNumber);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByEmpoyeeNumber: Employee Read by employeeNumber Complete = {employeeNumber}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeActions.ReadByEmpoyeeNumber: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), new Employee());
        }
    }

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmailAddress(string emailAddress)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmailAddress: Employee Read by emailAddress Starting = {emailAddress}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection).WhereEqualTo(EmployeeContants.emailAddress, emailAddress);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByEmailAddress: Employee Read by email Address Complete = {emailAddress}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeActions.ReadByEmailAddress: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), new Employee());
        }
    }

    async Task<DalResponse> IEmployeeActions.Save(string userUID, Employee employeeModel)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.Save: Starting read for employee UID - {employeeModel.employeeUID}");

            var db = Utilities.connectToFirestore();
            CollectionReference collection = db.Collection(EmployeeContants.employeeCollection);
            DocumentReference employeeRef = db.Collection(EmployeeContants.employeeCollection).Document(userUID);

            var result = await employeeRef.SetAsync(employeeModel);

            _logger.LogInformation($"EmployeeActions.Save: Finished read for employee {employeeModel.employeeUID}");

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.Save: Exception {ex.Message}");

            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }

    async Task<DalResponse> IEmployeeActions.SetAdminPrivilege(string employeeUID, bool adminPrivilege)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.SetAdminPrivilege: Starting read for employee UID - {employeeUID}");

            var db = Utilities.connectToFirestore();


            CollectionReference collection = db.Collection(EmployeeContants.employeeCollection);
            DocumentReference employeeRef = db.Collection(EmployeeContants.employeeCollection).Document(employeeUID);

            Dictionary<string, object> adminPrivilegeUpdate = new Dictionary<string, object>
            {
                { "adminPrivilege", adminPrivilege }
            };
            var result =  await employeeRef.UpdateAsync(adminPrivilegeUpdate);

            _logger.LogInformation($"EmployeeActions.SetAdminPrivilege: Finished updating admin privilege for {employeeUID} to {adminPrivilege}");

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.SetAdminPrivilege: Exception {ex.Message}");

            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }
}
