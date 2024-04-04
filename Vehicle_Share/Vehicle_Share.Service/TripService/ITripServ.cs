using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.TripService
{
    public interface ITripServ
    {
        Task<ResponseForOneModel<GetTripModel>> GetByIdAsync(string id);
        Task<GenResponseModel<GetTripModel>> GetAllForUserAsync();
        Task<GenResponseModel<GetTripDriverModel>> GetAllDriverTripAsync();
        Task<GenResponseModel<GetTripPassengerModel>> GetAllPassengerTripAsync();
        Task<ResponseModel> AddAsync(TripDriverModel model);
        Task<ResponseModel> AddAsync(TripPassengerModel model);
        Task<ResponseModel> UpdateAsync(string id, TripDriverModel model);
        Task<ResponseModel> UpdateAsync(string id, TripPassengerModel model);
        Task<int> DeleteAsync(string id);

        /* 
        Task<GenResponseModel<GetCarModel>> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, CarModel model);*/
    }
}
