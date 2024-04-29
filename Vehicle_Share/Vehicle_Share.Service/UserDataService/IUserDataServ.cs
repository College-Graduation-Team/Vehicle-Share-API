using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.UserDataService
{
    public interface IUserDataServ
    {
        Task<ResponseModel> GetUserDataAsync();
        Task<ResponseModel> GetUserDataByIdAsyc(string id);
        Task<ResponseModel> AddAndUpdateAsync(UserDataModel model);

        //  Task<ResponseModel> UpdateAsync(UserDataModel model);
        //  Task<int> DeleteAsync(string id);


    }
}
