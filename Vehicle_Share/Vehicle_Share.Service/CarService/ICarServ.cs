using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.CarService
{
    public interface ICarServ
    {
        Task<ResponseForOneModel<GetCarModel>> GetByIdAsync(string id);
        Task<GenResponseModel<GetCarModel>> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, UpdateCarModel model);
        Task<int> DeleteAsync(string id);
    }
}
