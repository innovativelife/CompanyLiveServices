using InnovativeLife.DataAccess.Common;

namespace InnovativeLife.DataAccess.User;

public interface IUserActions
{
    public Task<Tuple<DalResponse, UserModel?>> ReadByUID(string userUID);
    public Task<Tuple<DalResponse, UserModel?>> ReadByIdentifier(string identifier);
    public Task<Tuple<DalResponse, UserModel?>> Save(string userUID, UserModel userModel);
}