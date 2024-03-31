
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.LicenseService
{
    public interface ILicServ
    {
        Task<License> GetAllAsync();
        Task<string> AddAsync(LicModel model);
        Task<string> UpdateAsync(string id, LicModel model);
        Task DeleteAsync(License license);
    }
}
