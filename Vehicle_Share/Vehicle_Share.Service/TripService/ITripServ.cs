using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.TripService
{
    public interface ITripServ
    {
        Task<ResponseModel> GetByIdAsync(string id);
        Task<ResponseModel> GetAllForUserAsync();
        Task<ResponseModel> GetAllDriverTripAsync();
        Task<ResponseModel> GetAllPassengerTripAsync();
        Task<ResponseModel> AddAsync(TripDriverModel model);
        Task<ResponseModel> AddAsync(TripPassengerModel model);
        Task<ResponseModel> UpdateAsync(string id, UpdateTripDriverModel model);
        Task<ResponseModel> UpdateAsync(string id, UpdateTripPassengerModel model);
        Task<int> DeleteAsync(string id);

        /* 
        Task<GenResponseModel<GetCarModel>> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, CarModel model);*/
    }
}
