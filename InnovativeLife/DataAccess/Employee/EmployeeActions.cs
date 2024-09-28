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

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmployeeUID(string tenantId, string employeeUID)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmployeeUID: User Read by employeeUID Starting - {employeeUID}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection)
                .WhereEqualTo(EmployeeContants.tenantId, tenantId)
                .WhereEqualTo(EmployeeContants.employeeUID, employeeUID);
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

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmployeeNumber(string tenantId, string employeeNumber)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmpoyeeNumber: Employee Read by employeeNumber Starting = {employeeNumber}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection)
                .WhereEqualTo(EmployeeContants.tenantId, tenantId)
                .WhereEqualTo(EmployeeContants.employeeNumber, employeeNumber);
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

    async Task<Tuple<DalResponse, Employee?>> IEmployeeActions.ReadByEmail(string tenantId, string email)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.ReadByEmail: Employee Read by email Starting = {email}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection)
                .WhereEqualTo(EmployeeContants.tenantId, tenantId)
                .WhereEqualTo(EmployeeContants.email, email);
            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.NotFound), null);
            }

            var value = employeeQuerySnapshot[0].ConvertTo<Employee>();

            _logger.LogInformation($"EmployeeActions.ReadByEmail: Employee Read by email Complete = {email}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"EmployeeActions.ReadByEmail: Exception {ex.Message}");

            return new Tuple<DalResponse, Employee?>(new DalResponse(DalResponse.ResponseStatus.Ok), new Employee());
        }
    }

    async Task<Tuple<DalResponse, List<Employee>>> IEmployeeActions.Search(string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.Search starting with parameters: tenantId: {tenantId} | employeeNumber: {employeeNumber} | email: {email} | firstName: {firstName} | lastName: {lastName} | leaderEmployeeNumber: {leaderEmployeeNumber}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(EmployeeContants.employeeCollection);

            employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.tenantId, tenantId);

            if (!string.IsNullOrEmpty(employeeNumber))
            {
                employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.employeeNumber, employeeNumber);
            }

            if (!string.IsNullOrEmpty(email))
            {
                employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.email, email);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.firstName, firstName);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.lastName, lastName);
            }

            if (!string.IsNullOrEmpty(leaderEmployeeNumber))
            {
                employeeQuery = employeeQuery.WhereEqualTo(EmployeeContants.leaderEmployeeNumber, leaderEmployeeNumber);
            }

            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, List<Employee>>(new DalResponse(DalResponse.ResponseStatus.NotFound), new List<Employee>());
            }

            var employees = new List<Employee>();
            foreach (DocumentSnapshot documentSnapshot in employeeQuerySnapshot.Documents)
            {
                employees.Add(documentSnapshot.ConvertTo<Employee>());
            }

            _logger.LogInformation($"EmployeeActions.Search: Complete with {employees.Count} employees returned");

            return new Tuple<DalResponse, List<Employee>>(new DalResponse(DalResponse.ResponseStatus.Ok), employees);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.Save: Exception {ex.Message}");

            return new Tuple<DalResponse, List<Employee>>(new DalResponse(DalResponse.ResponseStatus.Ok), new List<Employee>());
        }
    }

    async Task<DalResponse> IEmployeeActions.Save(string tenantId, string userUID, Employee employeeModel)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.Save: Starting read for employee UID - {employeeModel.employeeUID}");

            var db = Utilities.connectToFirestore();
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

    async Task<DalResponse> IEmployeeActions.SetAdminPrivilege(string tenantId, string employeeUID, bool adminPrivilege)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.SetAdminPrivilege: Starting read for employee UID - {employeeUID}");

            var db = Utilities.connectToFirestore();

            DocumentReference employeeRef = db.Collection(EmployeeContants.employeeCollection).Document(employeeUID);

            Dictionary<string, object> adminPrivilegeUpdate = new Dictionary<string, object>
            {
                { EmployeeContants.adminPrivilege, adminPrivilege }
            };
            var result = await employeeRef.UpdateAsync(adminPrivilegeUpdate);

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
