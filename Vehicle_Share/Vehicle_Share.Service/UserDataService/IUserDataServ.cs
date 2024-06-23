using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.UserDataService
{
    public interface IUserDataServ
    {
       

        Task<ResponseModel> GetUserDataAsync();
        Task<ResponseModel> AddAndUpdateAsync(UserDataModel model);
        Task<ResponseModel> AddAndUpdateNationalImageAsync(NationalImageModel model);

        Task<ResponseModel> AddAndUpdateFCMTokenAsync(string token);
        Task<ResponseModel> AddRateAsync(string id, int rate);


        #region Admin

        Task<ResponseModel> GetAllUserAsync();
        Task<ResponseModel> GetUserByIdAsyc(string id);
        Task<ResponseModel> GetUserDataByUserIdAsync(string id);
        Task<ResponseModel> GetUserDataAllAsync();
        Task<ResponseModel> GetUserDataByIdAsyc(string id);
        Task<ResponseModel> UpdateAsync(string id, UserDataModel model);
        Task<ResponseModel> UpdateStatusRequestAsync(string id, UpdateStatusRequestModel model);

        #endregion
        //  Task<ResponseModel> UpdateAsync(UserDataModel model);
        //  Task<int> DeleteAsync(string id);


    }
}
