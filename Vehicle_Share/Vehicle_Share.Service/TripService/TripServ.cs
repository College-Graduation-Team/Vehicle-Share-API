using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.TripService
{
    public class TripServ : ITripServ
    {
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Car> _car;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IStringLocalizer<SharedResources> _LocaLizer;


        public TripServ(IBaseRepo<Trip> trip, IBaseRepo<UserData> userdata, IHttpContextAccessor httpContextAccessor, IBaseRepo<Car> car, IStringLocalizer<SharedResources> locaLizer = null)
        {
            _trip = trip;
            _userdata = userdata;
            _httpContextAccessor = httpContextAccessor;
            _car = car;
            _LocaLizer = locaLizer;
        }

        public async Task<ResponseForOneModel<GetTripModel>> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            var trip = await _trip.GetByIdAsync(id);

            if (trip is null)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoTrip] };

            var car = await _car.FindAsync(e => e.UserDataId == userData.Id);
            if (car is null)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoCar] };

            if (trip.AvailableSeats > car.Seats)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.InvalidAvailableSeats] };

            if (trip.RequestedSeats > trip.AvailableSeats)
                return new ResponseForOneModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.InvalidRequestedSeats] };

            var result = new ResponseForOneModel<GetTripModel>
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

        public async Task<GenResponseModel<GetTripModel>> GetAllForUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new GenResponseModel<GetTripModel>();

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

        public async Task<GenResponseModel<GetTripDriverModel>> GetAllDriverTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripDriverModel> { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripDriverModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();


            
            var userTrips = allTrips.Where(t => t.CarId is not null && t.IsFinished is false && t.AvailableSeats.Value > 0).ToList();

            var result = new GenResponseModel<GetTripDriverModel>();

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
                
                result.IsSuccess = true;
            }
            return result;
        }
        
        public async Task<GenResponseModel<GetTripPassengerModel>> GetAllPassengerTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripPassengerModel> { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripPassengerModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();

            var userTrips = allTrips.Where(t => t.CarId is null && t.IsFinished is false ).ToList();
            var result = new GenResponseModel<GetTripPassengerModel>();

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
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (model.Type is false) // Driver
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoDriver] };

            var car = await _car.FindAsync(e => e.Id == model.CarId);
            if (string.IsNullOrEmpty(model.CarId) || car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar] };

            Trip trip = new()
            {
                Id = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                AvailableSeats = model.AvailableSeats,
                RecommendedPrice = model.RecommendedPrice,
                CreatedOn=DateTime.UtcNow,

                // Relation
                UserDataId = userData.Id,
                CarId = model.CarId
            };

            await _trip.AddAsync(trip);


            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }

        public async Task<ResponseModel> AddAsync(TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (model.Type is true) // Passenger
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoPassanger] };
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
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };


        }

        public async Task<ResponseModel> UpdateAsync(string id, UpdateTripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (model.Type is false) // Driver
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoDriver] };

            var trip = await _trip.FindAsync(e => e.Id == id);

            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoDriver] };

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
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (model.Type is true) // Passenger
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoPassanger] };

            var trip = await _trip.FindAsync(e => e.Id == id);

            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip] };

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
