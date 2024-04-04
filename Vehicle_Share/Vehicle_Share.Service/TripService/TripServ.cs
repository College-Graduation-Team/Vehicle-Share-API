using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.TripService
{
    public class TripServ : ITripServ
    {
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Car> _car;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TripServ(IBaseRepo<Trip> trip, IBaseRepo<UserData> userdata, IHttpContextAccessor httpContextAccessor)
        {
            _trip = trip;
            _userdata = userdata;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseForOneModel<GetTripModel>> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = " User Not Authorized." };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = " User Not Added Data." };

            var trip = await _trip.GetByIdAsync(id);

            if (trip is null)
                return new ResponseForOneModel<GetTripModel> { ErrorMesssage = "Trip Not Found." };

            
            var result = new ResponseForOneModel<GetTripModel>
            {
                Data = new GetTripModel
                {
                    ID = trip.TripID,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    Recommendprice = trip.Recommendprice,
                    AvilableSets = trip.AvilableSets,
                    NumOfSetWant = trip.NumOfSetWant,
                    IsFinish = trip.IsFinish,
                    User_DataId = trip.User_DataId,
                    Car_Id = trip.Car_Id
                },
                IsSuccess = true
            };

            return result;
        }

        public async Task<GenResponseModel<GetTripModel>> GetAllForUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new GenResponseModel<GetTripModel> { ErrorMesssage = " User Not Added data  . " };

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new GenResponseModel<GetTripModel>();

            foreach (var trip in userTrips)
            {
                result.Data?.Add(new GetTripModel
                {
                    ID = trip.TripID,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    Recommendprice = trip.Recommendprice,
                    AvilableSets = trip.AvilableSets,    //driver
                    NumOfSetWant = trip.NumOfSetWant,  //passenger
                    IsFinish = trip.IsFinish,
                    User_DataId = trip.User_DataId,
                    Car_Id = trip.Car_Id
                });
            }
            result.IsSuccess = true;
            return result;
        }

        public async Task<GenResponseModel<GetTripDriverModel>> GetAllDriverTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = " User Not Added data  . " };

            var allTrips = await _trip.GetAllAsync();


            if (userData.typeOfUser is true)
                return new GenResponseModel<GetTripDriverModel> { ErrorMesssage = " You Can't show trips of drivers . " };

            var userTrips = allTrips.Where(t => t.Car_Id is not null).ToList();
            var result = new GenResponseModel<GetTripDriverModel>();

            foreach (var trip in userTrips)
            {
                if(!trip.IsFinish)
                result.Data?.Add(new GetTripDriverModel
                {
                    id = trip.TripID,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    Recommendprice = trip.Recommendprice,
                    AvilableSets = (int)trip.AvilableSets,
                    CarID = trip.Car_Id, // Assuming CarID is a string property
                    IsFinish=trip.IsFinish
                    
                });
            }
            result.IsSuccess = true;
            return result;
        }

        public async Task<GenResponseModel<GetTripPassengerModel>> GetAllPassengerTripAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = " User Not Added data  . " };

            var allTrips = await _trip.GetAllAsync();


            if (!userData.typeOfUser is true)
                return new GenResponseModel<GetTripPassengerModel> { ErrorMesssage = "You Can't show the trips of passenger . " };
            var userTrips = allTrips.Where(t => t.Car_Id is null).ToList();
            var result = new GenResponseModel<GetTripPassengerModel>();

            foreach (var trip in userTrips)
            {
                if (!trip.IsFinish)
                    result.Data?.Add(new GetTripPassengerModel
                    {
                        id = trip.TripID,
                        From = trip.From,
                        To = trip.To,
                        Date = trip.Date,
                        Recommendprice = trip.Recommendprice,
                        NumOfSetWant = (int)trip.NumOfSetWant,
                        Isfinish = trip.IsFinish

                    });
            }
            result.IsSuccess = true;
            return result;
        }
      
        public async Task<ResponseModel> AddAsync(TripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = "User not authorized" };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };
          
            if (userData.typeOfUser is false) // Driver
                return new ResponseModel { Messsage = "You are not driver " };

                Trip trip = new()
                {
                    TripID = Guid.NewGuid().ToString(),
                    From = model.From,
                    To = model.To,
                    Date = model.Date,
                    AvilableSets = model.AvilableSets,
                    Recommendprice = model.Recommendprice,

                    // Relation
                    User_DataId = userData.UserDataID,
                    Car_Id = model.CarID
                };

                await _trip.AddAsync(trip);
            
           
            return new ResponseModel { Messsage = " Trip add successfully " , IsSuccess=true };
        }
        
        public async Task<ResponseModel> AddAsync(TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = "User not authorized" };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            if (userData.typeOfUser is true) // Passenger
                return new ResponseModel { Messsage = "You are not Passenger " };
            Trip trip = new()
            {
                TripID = Guid.NewGuid().ToString(),
                From = model.From,
                To = model.To,
                Date = model.Date,
                NumOfSetWant = model.NumOfSetWant,
                Recommendprice = model.Recommendprice,

                // Relation
                User_DataId = userData.UserDataID
            };

            await _trip.AddAsync(trip);
            return new ResponseModel { Messsage = " Trip add successfully ", IsSuccess = true };


        }

        public async Task<ResponseModel> UpdateAsync(string id, TripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = "User not authorized" };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            if (userData.typeOfUser is false) // Driver
                return new ResponseModel { Messsage = "You are not driver " };

            var trip = await _trip.FindAsync(e => e.TripID == id);

            if (trip == null)
                return new ResponseModel { Messsage = "trip id is not found" };

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.AvilableSets = model.AvilableSets;
            trip.Recommendprice = model.Recommendprice;
            trip.Car_Id = model.CarID;
            
            return  new ResponseModel { Messsage = "Trip updated successfully", IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            if (userData.typeOfUser is true) // Passenger
                return new ResponseModel { Messsage = "You are not Passenger " };

            var trip = await _trip.FindAsync(e => e.TripID == id);

            if (trip == null)
                return new ResponseModel { Messsage = "trip id is not found" };

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.Recommendprice = model.Recommendprice;
            trip.NumOfSetWant = model.NumOfSetWant;

            /////////////////////////////////////////////////////////////

            return new ResponseModel { Messsage = "Trip updated successfully" , IsSuccess = true };
        }

        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var trip = await _trip.FindAsync(t => t.TripID == id);
            int res = await _trip.DeleteAsync(trip);
            return res;
        }

    }
}
