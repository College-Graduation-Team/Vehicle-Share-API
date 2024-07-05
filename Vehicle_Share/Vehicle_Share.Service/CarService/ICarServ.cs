using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.CarService
{
    public interface ICarServ
    {
        Task<ResponseModel> GetByIdAsync(string id);
        Task<ResponseModel> GetByUserDataIdAsync(string id);
        Task<ResponseModel> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, UpdateCarModel model);
        Task<ResponseModel> UpdateStatusRequestAsync(string id, UpdateStatusRequestModel model);
        Task<ResponseModel> DeleteAsync(string id);
        Task<ResponseModel> GetStatusAsync();

    }
}
