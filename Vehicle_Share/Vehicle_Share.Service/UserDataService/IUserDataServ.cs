using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.UserDataService
{
    public interface IUserDataServ
    {
        //  Task<IEnumerable<UserData>> GetAllAsyc();
        Task<ResponseForOneModel<GetUserModel>> GetUserDataAsync();
        Task<ResponseModel> AddAsync(UserDataModel model);
        Task<ResponseModel> UpdateAsync(string id, UserDataModel model);
        //  Task<int> DeleteAsync(string id);


    }
}
