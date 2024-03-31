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
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Repository.GenericRepo;
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

        public async Task<List<Trip>> GetAllForUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            var allTrips = await _trip.GetAllAsync();
            var userTrips = allTrips.Where(t => t.User_DataId == userData.UserDataID).ToList();

            return userTrips;
        }

        public async Task<List<GetTripDriverModel>> GetAllAsPassengerAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            var allTrips = await _trip.GetAllAsync();


            if (!userData.typeOfUser is true)
                return null;
            var userTrips = allTrips.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new List<GetTripDriverModel>();

            foreach (var trip in userTrips)
            {
                result.Add(new GetTripDriverModel
                {
                    id = trip.TripID,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    Recommendprice = trip.Recommendprice,
                    AvilableSets = (int)trip.AvilableSets,
                    CarID = trip.Car_Id // Assuming CarID is a string property
                });
            }

            return result;
        }

        public async Task<List<GetTripPassengerModel>> GetAllAsDriverAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            var allTrips = await _trip.GetAllAsync();


            if (userData.typeOfUser is true)
                return null;
            var userTrips = allTrips.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new List<GetTripPassengerModel>();

            foreach (var trip in userTrips)
            {
                result.Add(new GetTripPassengerModel
                {
                    id = trip.TripID,
                    From = trip.From,
                    To = trip.To,
                    Date = trip.Date,
                    Recommendprice = trip.Recommendprice,
                    NumOfSetWant=(int)trip.NumOfSetWant
                });
            }

            return result;
        }
      
        public async Task<string> AddAsync(TripDriverModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return "User not authorized";

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return "User not found";
          
            if (userData.typeOfUser is true) // Driver
                return "You are not driver ";


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
            
           
            return " Trip add successfully ";
        }
        
        public async Task<string> AddAsync(TripPassengerModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return "User not authorized";

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return "User not found";

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
            return " Trip add successfully ";


        }

        public async Task<string> UpdateAsync(string id, TripDriverModel model)
        {
            var trip = await _trip.FindAsync(e => e.TripID == id);

            if (trip == null)
                return "trip id is not found";

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.AvilableSets = model.AvilableSets;
            trip.Recommendprice = model.Recommendprice;
            trip.Car_Id = model.CarID;
            
            return "Trip updated successfully";
        }

        public async Task<string> UpdateAsync(string id, TripPassengerModel model)
        {
            var trip = await _trip.FindAsync(e => e.TripID == id);

            if (trip == null)
                return "trip id is not found";

           

            trip.From = model.From;
            trip.To = model.To;
            trip.Date = model.Date;
            trip.Recommendprice = model.Recommendprice;
            trip.NumOfSetWant = model.NumOfSetWant;

            /////////////////////////////////////////////////////////////

            return "Trip updated successfully";
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
