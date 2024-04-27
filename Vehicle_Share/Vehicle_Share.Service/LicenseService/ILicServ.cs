using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.LicenseService
{
    public interface ILicServ
    {
        Task<ResponseModel> GetLicenseAsync();
        Task<ResponseModel> AddAndUpdateAsync(LicModel model);
       // Task<ResponseModel> UpdateAsync(string id, UpdateLicModel model);
        Task<ResponseModel> DeleteAsync();
    }
}
