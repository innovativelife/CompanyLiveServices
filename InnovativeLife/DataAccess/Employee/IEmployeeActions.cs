using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Employee;

public interface IEmployeeActions
{
    public Task<Tuple<DalResponse, Employee?>> ReadByEmployeeUID(string tenantId,string employeeUID);
    public Task<Tuple<DalResponse, Employee?>> ReadByEmployeeNumber(string tenantId,string employeeNumber);
    public Task<Tuple<DalResponse, Employee?>> ReadByEmail(string tenantId,string employeeNumber);
    public Task<Tuple<DalResponse, List<Employee>>> Search(string tenantId, string? employeeNumber, string? email, string? firstName, string? lastName, string? leaderEmployeeNumber);
    public Task<DalResponse> Save(string tenantId,string userUID, Employee employeeModel);
    public Task<DalResponse> SetAdminPrivilege(string tenantId, string userUID, bool adminPrivilege);
}