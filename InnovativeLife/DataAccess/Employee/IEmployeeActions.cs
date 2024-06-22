using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.Employee;

public interface IEmployeeActions
{
    public Task<Tuple<DalResponse, Employee?>> ReadByUID(string userUID);
    public Task<Tuple<DalResponse, Employee?>> ReadByIdentifier(string identifier);
    public Task<Tuple<DalResponse, Employee?>> Save(string userUID, Employee employeeModel);
}