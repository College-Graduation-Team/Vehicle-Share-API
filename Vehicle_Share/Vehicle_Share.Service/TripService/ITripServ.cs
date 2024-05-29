using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.TripService
{
    public interface ITripServ
    {
        Task<ResponseModel> SearchDriverTripAsync(SearchModel model);
        Task<ResponseModel> SearchPassengerTripAsync(SearchModel model);
        Task<ResponseModel> GetByIdAsync(string id);
        Task<ResponseModel> GetAllForUserAsDriverAsync(bool IsFinished);
        Task<ResponseModel> GetAllForUserAsPassengerAsync(bool IsFinished);
        Task<ResponseModel> GetAllDriverTripAsync();
        Task<ResponseModel> GetAllPassengerTripAsync();

        Task<ResponseModel> AddAsync(TripDriverModel model);
        Task<ResponseModel> AddAsync(TripPassengerModel model);

        Task<ResponseModel> UpdateAsync(string id, UpdateTripDriverModel model);
        Task<ResponseModel> UpdateAsync(string id, UpdateTripPassengerModel model);

        Task<ResponseModel> TripFinishedAsync(string id);

        Task<int> DeleteAsync(string id);

        #region Dashboard
        Task<ResponseModel> GetTripByUserDataIdAsync(string id);
        Task<ResponseModel> GetAllTripAsync();

        #endregion

        /* 
        Task<GenResponseModel<GetCarModel>> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, CarModel model);*/
    }
}
