using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;
using Twilio.Http;

namespace Vehicle_Share.Service.TripService
{
    public class TripServ : ITripServ
    {
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Car> _car;
        private readonly IBaseRepo<License> _lic;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IStringLocalizer<SharedResources> _LocaLizer;


        public TripServ(IBaseRepo<Trip> trip, IBaseRepo<UserData> userdata, IHttpContextAccessor httpContextAccessor, IBaseRepo<Car> car, IStringLocalizer<SharedResources> locaLizer = null, IBaseRepo<License> lic = null)
        {
            _trip = trip;
            _userdata = userdata;
            _httpContextAccessor = httpContextAccessor;
            _car = car;
            _LocaLizer = locaLizer;
            _lic = lic;
        }

        public async Task<ResponseModel> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] ,code=ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var trip = await _trip.GetByIdAsync(id);

            if (trip is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };

            var result = new ResponseDataModel<GetTripModel>
            {
                data = new GetTripModel
                {
                    Id = trip.Id,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,
                    RequestedSeats = trip.RequestedSeats,
                    CreatedOn=trip.CreatedOn,
                    IsFinished = trip.IsFinished,
                    UserDataId = trip.UserDataId,
                    CarId = trip.CarId
                    
                },
                IsSuccess = true
            };

            return result;
        }

        public async Task<ResponseModel> GetAllForUserAsDriverAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.CarId is not null && t.UserDataId == userData.Id).ToList();
            var result = new ResponseDataModel<List<GetTripModel>>();
            result.data = new List<GetTripModel>();
            foreach (var trip in userTrips)
            {
                result.data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,    //driver
                    RequestedSeats = trip.RequestedSeats,  //passenger
                    IsFinished = trip.IsFinished,
                    UserDataId = trip.UserDataId,
                    CarId = trip.CarId,
                });
            }

            result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> GetAllForUserAsPassengerAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.CarId is null && t.UserDataId == userData.Id).ToList();
            var result = new ResponseDataModel<List<GetTripModel>>();
            result.data = new List<GetTripModel>();
            foreach (var trip in userTrips)
            {
                result.data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,    //driver
                    RequestedSeats = trip.RequestedSeats,  //passenger
                    IsFinished = trip.IsFinished,
                    UserDataId = trip.UserDataId,
                    CarId = trip.CarId,
                });
            }

            result.IsSuccess = true;
            return result;
        }

        public async Task<ResponseModel> GetAllDriverTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();



            var userTrips = allTrips.Where(t => t.CarId is not null && t.IsFinished is false && t.AvailableSeats.Value > 0 && t.UserDataId != userData.Id).ToList();

            var result = new ResponseDataModel<List<GetTripDriverModel>>();
            result.data =new List<GetTripDriverModel>();
            foreach (var trip in userTrips)
            {
                var car = await _car.FindAsync(e => e.Id == trip.CarId);
                
                    result.data?.Add(new GetTripDriverModel
                    {
                        Id = trip.Id,
                        From = trip.From,
                        To = trip.To,
                        Date = trip.Date,
                        RecommendedPrice = trip.RecommendedPrice,
                        AvailableSeats = trip.AvailableSeats.Value, // Access the Value property
                        CreatedOn=trip.CreatedOn,
                        CarId = trip.CarId, // Assuming CarID is a string property
                        CarType = car.Type,
                        CarBrand = car.Brand,
                    });
                
            }
            result.IsSuccess = true;
            return result;
        }
        
        public async Task<ResponseModel> GetAllPassengerTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();

            var userTrips = allTrips.Where(t => t.CarId is null && t.IsFinished is false && t.UserDataId != userData.Id).ToList();
            var result = new ResponseDataModel<List<GetTripPassengerModel>>();
            result.data = new List<GetTripPassengerModel>();
            foreach (var trip in userTrips)
            {
                    result.data?.Add(new GetTripPassengerModel
                    {
                        Id = trip.Id,
                        From = trip.From,
                        To = trip.To,
                        Date = trip.Date,
                        RecommendedPrice = trip.RecommendedPrice,
                        RequestedSeats = trip.RequestedSeats.Value,
                        CreatedOn=trip.CreatedOn,
                        IsFinished = trip.IsFinished
                    });

            }
            result.IsSuccess = true;
            return result;
        }

        public async Task<ResponseModel> AddAsync(TripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };


            var car = await _car.FindAsync(e => e.Id == model.CarId);
            if (string.IsNullOrEmpty(model.CarId) || car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            var lic = await _lic.FindAsync(e=>e.UserDataId==userData.Id);
            if (lic is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoLicense], code = ResponseCode.NoLicense };

            Trip trip = new()
            {
                Id = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                AvailableSeats = model.AvailableSeats,
                RecommendedPrice = model.RecommendedPrice,
                CreatedOn= DateTime.UtcNow,

                // Relation
                UserDataId = userData.Id,
                CarId = model.CarId
            };

            await _trip.AddAsync(trip);

            var result = new ResponseDataModel<UserDataResponseModel>
            {
                message = _LocaLizer[SharedResourcesKey.Created],
                IsSuccess = true,
                data = new UserDataResponseModel { Id = trip.Id, UserDataId = userData.Id }
            };
            return result;
        }

        public async Task<ResponseModel> AddAsync(TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            
            Trip trip = new()
            {
                Id = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                RequestedSeats = model.RequestedSeats,
                RecommendedPrice = model.RecommendedPrice,

                // Relation
                UserDataId = userData.Id
            };

            await _trip.AddAsync(trip);

            var result = new ResponseDataModel<UserDataResponseModel>
            {
                message = _LocaLizer[SharedResourcesKey.Created],
                IsSuccess = true,
                data = new UserDataResponseModel { Id = trip.Id, UserDataId = userData.Id }
            };
            return result;
        }

        public async Task<ResponseModel> UpdateAsync(string id, UpdateTripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var lic = await _lic.FindAsync(e => e.UserDataId == userData.Id);
            if (lic is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoLicense] , code = ResponseCode.NoLicense };

            var trip = await _trip.FindAsync(e => e.Id == id);
            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip] , code = ResponseCode.NoTrip };

            trip.From = model.From ??trip.From;
            trip.To = model.To ?? trip.To;
            trip.Date = model.Date !=null ? model.Date : trip.Date;
            trip.RecommendedPrice = model.RecommendedPrice > 0 ? model.RecommendedPrice : trip.RecommendedPrice;
            trip.AvailableSeats = model.AvailableSeats > 0 ? model.AvailableSeats : trip.AvailableSeats;
            trip.CarId = model.CarId ?? trip.CarId;

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, UpdateTripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };


            var trip = await _trip.FindAsync(e => e.Id == id);

            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip] , code = ResponseCode.NoTrip };

            trip.From = model.From ?? trip.From;
            trip.To = model.To ?? trip.To;
            trip.Date = model.Date != null ? model.Date : trip.Date;
            trip.RecommendedPrice = model.RecommendedPrice > 0 ? model.RecommendedPrice : trip.RecommendedPrice;
            trip.RequestedSeats = model.RequestedSeats > 0 ? model.RequestedSeats : trip.RequestedSeats;

            /////////////////////////////////////////////////////////////

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
        }

        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var trip = await _trip.FindAsync(t => t.Id == id);
            int res = await _trip.DeleteAsync(trip);
            return res;
        }

    }
}
