﻿using Microsoft.AspNetCore.Http;
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
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoUserData] };

            var trip = await _trip.GetByIdAsync(id);

            if (trip is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoTrip] };

            var car = await _car.FindAsync(e => e.UserDataId == userData.Id);
            if (car is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoCar] };

            if (trip.AvailableSeats > car.Seats)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.InvalidAvailableSeats] };

            if (trip.RequestedSeats > trip.AvailableSeats)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.InvalidRequestedSeats] };

            var result = new ResponseForOneModel<GetTripModel>
            {
                Data = new GetTripModel
                {
                    Id = trip.Id,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    RecommendPrice = trip.RecommendPrice,
                    AvailableSeats = trip.AvailableSeats,
                    RequestedSeats = trip.RequestedSeats,
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
                return new GenResponseModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new GenResponseModel<GetTripModel>();

            foreach (var trip in userTrips)
            {
                result.Data?.Add(new GetTripModel
                {
                    Id = trip.Id,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    RecommendPrice = trip.RecommendPrice,
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
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();


            if (userData.Type is true)
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.ShowTripDriver] };

            var userTrips = allTrips.Where(t => t.CarId is not null).ToList();

            var result = new GenResponseModel<GetTripDriverModel>();

            foreach (var trip in userTrips)
            {
                var car = await _car.FindAsync(e => e.Id == trip.CarId);
                if (!trip.IsFinished && trip.AvailableSeats.HasValue) // Check if AvailableSeats has a value
                {
                    result.Data?.Add(new GetTripDriverModel
                    {
                        Id = trip.Id,
                        From = trip.From,
                        To = trip.To,
                        Date = trip.Date,
                        RecommendPrice = trip.RecommendPrice,
                        AvailableSeats = trip.AvailableSeats.Value, // Access the Value property
                        CarId = trip.CarId, // Assuming CarID is a string property
                        CarType = car.Type,
                        CarBrand = car.Brand,
                    });
                }
                result.IsSuccess = true;
            }
            return result;
        }
        public async Task<GenResponseModel<GetTripPassengerModel>> GetAllPassengerTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoUserData] };

            var allTrips = await _trip.GetAllAsync();

            if (!userData.Type is true)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.ShowTripPassanger] };
            var userTrips = allTrips.Where(t => t.CarId is null).ToList();
            var result = new GenResponseModel<GetTripPassengerModel>();

            foreach (var trip in userTrips)
            {
                if (!trip.IsFinished)
                    result.Data?.Add(new GetTripPassengerModel
                    {
                        Id = trip.Id,
                        From = trip.From,
                        To = trip.To,
                        Date = trip.Date,
                        RecommendPrice = trip.RecommendPrice,
                        RequestedSeats = trip.RequestedSeats.Value,
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
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (userData.Type is false) // Driver
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoDriver] };
            if (string.IsNullOrEmpty(model.CarId))
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoCar] };

            Trip trip = new()
            {
                Id = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                AvailableSeats = model.AvailableSeats,
                RecommendPrice = model.RecommendPrice,

                // Relation
                UserDataId = userData.Id,
                CarId = model.CarId
            };

            await _trip.AddAsync(trip);


            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }

        public async Task<ResponseModel> AddAsync(TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (userData.Type is true) // Passenger
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoPassanger] };
            Trip trip = new()
            {
                Id = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                RequestedSeats = model.RequestedSeats,
                RecommendPrice = model.RecommendPrice,

                // Relation
                UserDataId = userData.Id
            };

            await _trip.AddAsync(trip);
            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };


        }

        public async Task<ResponseModel> UpdateAsync(string id, TripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (userData.Type is false) // Driver
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoDriver] };

            var trip = await _trip.FindAsync(e => e.Id == id);

            if (trip == null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoDriver] };

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.AvailableSeats = model.AvailableSeats;
            trip.RecommendPrice = model.RecommendPrice;
            trip.CarId = model.CarId;

            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoUserData] };

            if (userData.Type is true) // Passenger
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoPassanger] };

            var trip = await _trip.FindAsync(e => e.Id == id);

            if (trip == null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoTrip] };

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.RecommendPrice = model.RecommendPrice;
            trip.RequestedSeats = model.RequestedSeats;

            /////////////////////////////////////////////////////////////

            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
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
