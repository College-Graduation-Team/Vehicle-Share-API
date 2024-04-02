using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.LicenseService
{
    public interface ILicServ
    {
        Task<ResponseForOneModel<GetLicModel>> GetAsync();
        Task<ResponseModel> AddAsync(LicModel model);
        Task<ResponseModel> UpdateAsync(string id, LicModel model);
        Task <int> DeleteAsync(string id);
    }
}
