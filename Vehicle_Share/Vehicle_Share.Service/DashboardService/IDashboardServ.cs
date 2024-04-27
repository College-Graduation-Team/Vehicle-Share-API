
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.DashboardService
{
    public interface IDashboardServ
    {

        Task<ResponseModel> GetAllUserDataAsync();
        Task<ResponseModel> GetUserDataByIdAsync(string id);
        Task<ResponseModel> UpdateUserData(string id,UserDataModel model);


        Task<ResponseModel> GetAllCarAsync();
        Task<ResponseModel> GetCarByIdAsync(string id);
        Task<ResponseModel> UpdateCar(string id, UpdateCarModel model);


        Task<ResponseModel> GetAllLicenseAsync();
        Task<ResponseModel> GetLicenseByIdAsync(string id);
        Task<ResponseModel> UpdateLicense(string id, LicModel model);
        

    }
}
