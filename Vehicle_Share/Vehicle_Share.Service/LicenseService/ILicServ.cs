using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.LicenseService
{
    public interface ILicServ
    {
        Task<ResponseModel> GetLicenseAsync();
        Task<ResponseModel> AddAndUpdateAsync(LicModel model);
        Task<ResponseModel> DeleteAsync();
        // Task<ResponseModel> UpdateAsync(string id, UpdateLicModel model);
        Task<ResponseModel> seedAsync(LicSeedModel model);

        Task<ResponseModel> GetAllAsync();
        Task<ResponseModel> GetUserDataByIdAsyc(string id);
        Task<ResponseModel> UpdateAsync(string id, LicModel model);
        Task<ResponseModel> UpdateStatusRequestAsync(string id, UpdateStatusRequestModel model);

    }
}
