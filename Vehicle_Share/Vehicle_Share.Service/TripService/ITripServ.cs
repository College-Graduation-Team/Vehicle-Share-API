using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.TripService
{
    public interface ITripServ
    {
        Task<List<Trip>> GetAllForUserAsync();
        Task<List<GetTripDriverModel>> GetAllAsPassengerAsync();
        Task<List<GetTripPassengerModel>> GetAllAsDriverAsync();
        Task<string> AddAsync(TripDriverModel model);
        Task<string> AddAsync(TripPassengerModel model);
        Task<string> UpdateAsync(string id, TripDriverModel model);
        Task<string> UpdateAsync(string id, TripPassengerModel model);
        Task<int> DeleteAsync(string id);

        /*  Task<ResponseForOneModel<GetCarModel>> GetByIdAsync(string id);
        Task<GenResponseModel<GetCarModel>> GetAllAsync();
        Task<ResponseModel> AddAsync(CarModel model);
        Task<ResponseModel> UpdateAsync(string id, CarModel model);*/
    }
}
