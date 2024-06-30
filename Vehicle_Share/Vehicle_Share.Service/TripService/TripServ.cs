﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Vehicle_Share.Service.TripService
{
    public class TripServ : ITripServ
    {
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Car> _car;
        private readonly IBaseRepo<License> _lic;
        private readonly IBaseRepo<Request> _request;

        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IStringLocalizer<SharedResources> _LocaLizer;


        public TripServ(IBaseRepo<Trip> trip, IBaseRepo<UserData> userdata, IHttpContextAccessor httpContextAccessor, IBaseRepo<Car> car, IStringLocalizer<SharedResources> locaLizer = null, IBaseRepo<License> lic = null, IBaseRepo<EF.Models.Request> request = null)
        {
            _trip = trip;
            _userdata = userdata;
            _httpContextAccessor = httpContextAccessor;
            _car = car;
            _LocaLizer = locaLizer;
            _lic = lic;
            _request = request;
        }

        public async Task<ResponseModel> SearchDriverTripAsync(SearchModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };


            // Define the fixed maximum distance (in kilometers)
            const double maxDistance = 50; // Adjust this value as needed
            // Get all trips
            var allTrips = await _trip.GetAllAsync();

            // Filter trips within the fixed maximum distance from either the 'from' or 'to' location
            var nearbyTrips = allTrips.Where(trip =>
            {
                bool fromMatch = model.FromLatitude.HasValue && model.FromLongitude.HasValue &&
                           CalculateDistance(model.FromLatitude.Value, model.FromLongitude.Value, trip.FromLatitude, trip.FromLongitude) <= maxDistance;
                bool toMatch = model.ToLatitude.HasValue && model.FromLongitude.HasValue &&
                               CalculateDistance(model.ToLatitude.Value, model.ToLongitude.Value, trip.ToLatitude, trip.ToLongitude) <= maxDistance;

                return (fromMatch || toMatch)
                && (!model.StartDate.HasValue || trip.Date >= model.StartDate)
                && (!model.EndDate.HasValue || trip.Date <= model.EndDate)
                && trip.CarId is not null
                && !trip.IsFinished && trip.AvailableSeats.HasValue
                && trip.UserDataId != userData.Id
                && trip.AvailableSeats.Value > 0;
            }
            ).ToList();
            var result = new ResponseDataModel<List<GetTripDriverModel>>();
            result.data = new List<GetTripDriverModel>();
            foreach (var trip in nearbyTrips)
            {
                var car = await _car.FindAsync(e => e.Id == trip.CarId);

                result.data?.Add(new GetTripDriverModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats.Value, // Access the Value property
                    CreatedOn = trip.CreatedOn,
                    CarId = trip.CarId, // Assuming CarID is a string property
                    CarType = car.Type,
                    CarBrand = car.Brand,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

                });

            }
            result.IsSuccess = true;
            return result;
        }
       
        public async Task<ResponseModel> SearchPassengerTripAsync(SearchModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };


            // Define the fixed maximum distance (in kilometers)
            const double maxDistance = 50; // Adjust this value as needed
            // Get all trips
            var allTrips = await _trip.GetAllAsync();

            // Filter trips within the fixed maximum distance from either the 'from' or 'to' location
            var nearbyTrips = allTrips.Where(trip =>
            {
                bool fromMatch = model.FromLatitude.HasValue && model.FromLongitude.HasValue &&
                           CalculateDistance(model.FromLatitude.Value, model.FromLongitude.Value, trip.FromLatitude, trip.FromLongitude) <= maxDistance;
                bool toMatch = model.ToLatitude.HasValue && model.FromLongitude.HasValue &&
                               CalculateDistance(model.ToLatitude.Value, model.ToLongitude.Value, trip.ToLatitude, trip.ToLongitude) <= maxDistance;

                return (fromMatch || toMatch)
                        && trip.CarId is null && !trip.IsFinished
                        && (!model.StartDate.HasValue || trip.Date >= model.StartDate)
                        && (!model.EndDate.HasValue || trip.Date <= model.EndDate)
                        && trip.UserDataId != userData.Id;
            }
            ).ToList();
            var result = new ResponseDataModel<List<GetTripPassengerModel>>();
            result.data = new List<GetTripPassengerModel>();
            foreach (var trip in nearbyTrips)
            {
                result.data?.Add(new GetTripPassengerModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    RequestedSeats = trip.RequestedSeats.Value,
                    CreatedOn = trip.CreatedOn,
                    IsFinished = trip.IsFinished,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

                });

            }
            result.IsSuccess = true;
            return result;
        }
      
        #region AllGet

        public async Task<ResponseModel> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] ,code=ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.UserId == userId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }

            var trip = await _trip.GetByIdAsync(id);

            if (trip is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };

            var user = await _userdata.FindAsync(u => u.Id == trip.UserDataId);

            var result = new ResponseDataModel<GetTripByIdModel>
            {
                data = new GetTripByIdModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,
                    RequestedSeats = trip.RequestedSeats,
                    CreatedOn=trip.CreatedOn,
                    IsFinished = trip.IsFinished,
                    CarId = trip.CarId,

                    UserDataId = user.Id,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,


                },
                IsSuccess = true
            };

            return result;
        }

        public async Task<ResponseModel> GetAllForUserAsDriverAsync(bool IsFinished)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();

            var userTrips = allTrips.Where(t => t.CarId is not null && t.UserDataId == userData.Id && (IsFinished ? t.IsFinished && t.DailySchedule == 0 : !t.IsFinished || t.DailySchedule > 0)).ToList();
           
            
            var result = new ResponseDataModel<List<GetTripModel>>();
            result.data = new List<GetTripModel>();
            foreach (var trip in userTrips)
            {
                
                result.data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,    //driver
                    RequestedSeats = trip.RequestedSeats,  //passenger
                    IsFinished = trip.IsFinished,
                    UserDataId = trip.UserDataId,
                    CarId = trip.CarId,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

                });
            }

            result.IsSuccess = true;
            return result;
        }
      
        public async Task<ResponseModel> GetAllForUserAsPassengerAsync(bool IsFinished)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.CarId is null && t.UserDataId == userData.Id && (IsFinished ? t.IsFinished && t.DailySchedule == 0 : !t.IsFinished || t.DailySchedule > 0)).ToList();
            var result = new ResponseDataModel<List<GetTripModel>>();
            result.data = new List<GetTripModel>();
            foreach (var trip in userTrips)
            {
                result.data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,    //driver
                    RequestedSeats = trip.RequestedSeats,  //passenger
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

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
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var allTrips = await _trip.GetAllAsync(
                     include: q => q.Include(trip => trip.Car)
                 );

            var userTrips = allTrips
                .Where(t => t.CarId != null && !t.IsFinished && t.AvailableSeats.Value > 0 && t.UserDataId != userData.Id)
                .ToList();

            var result = new ResponseDataModel<List<GetTripDriverModel>>();
            result.data = new List<GetTripDriverModel>();
            foreach (var trip in userTrips)
            {
                result.data?.Add(new GetTripDriverModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,

                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats.Value, // Access the Value property
                    CreatedOn = trip.CreatedOn,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

                    CarId = trip.CarId, // Assuming CarID is a string property
                    CarType = trip.Car?.Type,
                    CarBrand = trip.Car?.Brand,

                    UserDataId = trip.UserDataId,
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

                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,

                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    RequestedSeats = trip.RequestedSeats.Value,
                    CreatedOn = trip.CreatedOn,
                    IsFinished = trip.IsFinished,
                    DailySchedule = trip.DailySchedule,
                    Route = trip.Route,

                    UserDataId = trip.UserDataId,
                });

            }
            result.IsSuccess = true;
            return result;
        }

        public async Task<ResponseModel> GetAllUserDataIdsInTripAsync(string tripId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
           
            var trip = await _trip.GetByIdAsync(tripId);
            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };
          
            var userTrips = await _request.GetAllAsync(r => r.TripId == trip.Id && r.Status==Core.Helper.StatusContainer.Status.Accepted);

            var result = new ResponseDataModel<UserIdsModel> { data = new() { passengers = new() } };

            foreach (var user in userTrips)
            {
                
                if (user.Type) result.data.driver = user.UserDataId;
                else result.data.passengers.Add(user.UserDataId);
            }

            if (string.IsNullOrEmpty(result.data.driver)) result.data.driver = trip.UserDataId;
            else result.data.passengers.Add(trip.UserDataId);
            
            result.IsSuccess = true;
            return result;
        }
        #endregion

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
                FromLatitude = model.FromLatitude,
                FromLongitude = model.FromLongitude,
                ToLatitude = model.ToLatitude,
                ToLongitude = model.ToLongitude,
                Date = model.Date,
                AvailableSeats = model.AvailableSeats,
                RecommendedPrice = model.RecommendedPrice,
                CreatedOn = DateTime.UtcNow,
                DailySchedule = model.DailySchedule,
                Route = model.Route,
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
                FromLatitude = model.FromLatitude,
                FromLongitude = model.FromLongitude,
                ToLatitude = model.ToLatitude,
                ToLongitude = model.ToLongitude,
                Date = model.Date,
                RequestedSeats = model.RequestedSeats,
                RecommendedPrice = model.RecommendedPrice,
                DailySchedule = model.DailySchedule,
                Route = model.Route,


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

            trip.FromLatitude = model.FromLatitude ?? trip.FromLatitude;
            trip.FromLongitude = model.FromLongitude ?? trip.FromLongitude;
            trip.ToLatitude = model.ToLatitude ?? trip.ToLatitude;
            trip.ToLongitude = model.ToLongitude ?? trip.ToLongitude;

            trip.Date = model.Date != null ? DateTime.Parse(model.Date) : trip.Date;
            trip.RecommendedPrice = model.RecommendedPrice > 0 ? (float)model.RecommendedPrice : trip.RecommendedPrice;
            trip.AvailableSeats = model.AvailableSeats > 0 ? model.AvailableSeats : trip.AvailableSeats;
            trip.DailySchedule = model.DailySchedule;
            trip.Route = model.Route > 0 ? model.Route : trip.Route;
            trip.CarId = model.CarId ?? trip.CarId;

            await _trip.UpdateAsync(trip);

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

            trip.FromLatitude = model.FromLatitude ?? trip.FromLatitude;
            trip.FromLongitude = model.FromLongitude ?? trip.FromLongitude;
            trip.ToLatitude = model.ToLatitude ?? trip.ToLatitude;
            trip.ToLongitude = model.ToLongitude ?? trip.ToLongitude;

            trip.Date = model.Date != null ? DateTime.Parse(model.Date) : trip.Date;
            trip.RecommendedPrice = model.RecommendedPrice > 0 ? (float)model.RecommendedPrice : trip.RecommendedPrice;
            trip.RequestedSeats = model.RequestedSeats > 0 ? model.RequestedSeats : trip.RequestedSeats;
            trip.DailySchedule = model.DailySchedule;
            trip.Route = model.Route > 0 ? model.Route : trip.Route;

            /////////////////////////////////////////////////////////////
            await _trip.UpdateAsync(trip);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
        }

        public async Task<ResponseModel> TripFinishedAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var trip = await _trip.GetByIdAsync(id);
            if (trip == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };
            // if trip not started can do this    
            //

            trip.IsFinished = true;

            await _trip.UpdateAsync(trip);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.TripFinished], IsSuccess = true };

        }
        
        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var trip = await _trip.FindAsync(t => t.Id == id);
            int res = await _trip.DeleteAsync(trip);
            return res;
        }


        #region Dashboard
        public async Task<ResponseModel> GetTripByUserDataIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var trip = await _trip.FindAsync(e => e.UserDataId == id);

            if (trip is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };

            var user = await _userdata.FindAsync(u => u.Id == trip.UserDataId);

            var result = new ResponseDataModel<GetTripByIdModel>
            {
                data = new GetTripByIdModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
                    Date = trip.Date,
                    RecommendedPrice = trip.RecommendedPrice,
                    AvailableSeats = trip.AvailableSeats,
                    RequestedSeats = trip.RequestedSeats,
                    CreatedOn = trip.CreatedOn,
                    IsFinished = trip.IsFinished,
                    CarId = trip.CarId,

                    UserDataId = user.Id,

                },
                IsSuccess = true
            };

            return result;
        }

        public async Task<ResponseModel> GetAllTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var allTrips = await _trip.GetAllAsync();

            var result = new ResponseDataModel<List<GetTripModel>>();
            result.data = new List<GetTripModel>();
            foreach (var trip in allTrips)
            {
                result.data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    FromLatitude = trip.FromLatitude,
                    FromLongitude = trip.FromLongitude,
                    ToLatitude = trip.ToLatitude,
                    ToLongitude = trip.ToLongitude,
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

        #endregion


        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Radius of the Earth in kilometers
            double earthRadius = 6371;

            // Convert degrees to radians
            lat1 = Math.PI * lat1 / 180.0;
            lon1 = Math.PI * lon1 / 180.0;
            lat2 = Math.PI * lat2 / 180.0;
            lon2 = Math.PI * lon2 / 180.0;

            // Calculate differences
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            // Calculate distance using Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadius * c;

            return distance;
        }
   
    }
}
