﻿using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.TripService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripController : ControllerBase
    {
        private readonly ITripServ _service;
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _user;
        private readonly IBaseRepo<Car> _car;


        public TripController(ITripServ service, IBaseRepo<Trip> trip, IBaseRepo<UserData> user, IBaseRepo<Car> car)
        {
            _service = service;
            _trip = trip;
            _user = user;
            _car = car;
        }
        #region Get Requests


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is ResponseDataModel<GetTripByIdModel> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }
       
        [HttpGet("MyTripDriver")]
        public async Task<IActionResult> GetAllForUserAsDriverAsync(bool IsFinished)
        {
            var result = await _service.GetAllForUserAsDriverAsync(IsFinished);

            if (result is ResponseDataModel<List<GetTripModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }
      
        [HttpGet("MyTripPassenger")]
        public async Task<IActionResult> GetAllForUserAsPassengerAsync(bool IsFinished)
        {
            var result = await _service.GetAllForUserAsPassengerAsync(IsFinished);

            if (result is ResponseDataModel<List<GetTripModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpGet("driver")]
        public async Task<IActionResult> GetAllDriverTripAsync()
        {
            var result = await _service.GetAllDriverTripAsync();
            if (result is ResponseDataModel<List<GetTripDriverModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpGet("passenger")]
        public async Task<IActionResult> GetAllPassengerTripAsync()
        {

            var result = await _service.GetAllPassengerTripAsync();
            if (result is ResponseDataModel<List<GetTripPassengerModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }


        [HttpGet("search/driver")]
        public async Task<IActionResult> SearchDriverTripAsync([FromQuery] SearchModel model)
        {
            var result = await _service.SearchDriverTripAsync(model);
            if (result is ResponseDataModel<List<GetTripDriverModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }
        [HttpGet("search/passenger")]
        public async Task<IActionResult> SearchPassengerTripAsync([FromQuery] SearchModel model)
        {
            var result = await _service.SearchPassengerTripAsync(model);
            if (result is ResponseDataModel<List<GetTripPassengerModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        #endregion

        #region Post Requsets

        [HttpPost("driver")] // driver add trip 
        public async Task<IActionResult> AddTripAsync([FromBody] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAsync(model);
            if (result is ResponseDataModel<UserDataResponseModel> res)
                return Ok(new { res.message, res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPost("passenger")] //passenger add trip
        public async Task<IActionResult> AddTripAsync([FromBody] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAsync(model);
            if (result is ResponseDataModel<UserDataResponseModel> res)
                return Ok(new { res.message, res.data });

            return BadRequest(new { result.code, result.message });
        }

        #endregion

        #region Put and Delete Requsets

        [HttpPut("driver/{id}")] //update for driver
        public async Task<IActionResult> UpdataTripAsync([FromRoute] string id, [FromBody] UpdateTripDriverModel model)
        {

            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPut("passenger/{id}")]//update for Passenger
        public async Task<IActionResult> UpdataTripAsync([FromRoute] string id, [FromBody] UpdateTripPassengerModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTripAsync([FromRoute] string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion

        #region Seed fake data 
       
        [HttpPost("generate-fake-trip-Driver")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateFakeTrips(int count)
        {
            var usercar = new List<UserData>();
            var cars = new List<Car>();
            var users = await _user.GetAllAsync();
            //    var lic = await _Lic.GetAllAsync();
            foreach (var use in users)
            {
                var car = await _car.FindAsync(e => e.UserDataId == use.Id);
                if (car is null) continue;

                var x = await _user.GetByIdAsync(car.UserDataId);
                usercar.Add(use);
                cars.Add(car);
            }
            var nonAdminUsers = usercar.Where(u => u.Name != "Admin").ToList();

            // Generate fake trip data using Bogus
            var faker = new Faker<SeedTripDriverModel>()
                .RuleFor(t => t.FromLatitude, f => f.Address.Latitude())
                .RuleFor(t => t.FromLongitude, f => f.Address.Longitude())
                .RuleFor(t => t.ToLatitude, f => f.Address.Latitude())
                .RuleFor(t => t.ToLongitude, f => f.Address.Longitude())
                .RuleFor(t => t.Date, f => f.Date.Future())
                .RuleFor(t => t.RecommendedPrice, f => f.Random.Float(10, 1000))
                .RuleFor(t => t.AvailableSeats, f => f.Random.Short(1, 3)) // Assuming driver can have 1-5 available seats
                .RuleFor(t => t.CarId, f => cars[(f.UniqueIndex) % (nonAdminUsers.Count)].Id) // Assuming you generate a random car ID
                .RuleFor(t => t.userdataId, f => usercar[(f.UniqueIndex) % (nonAdminUsers.Count)].Id); // Pick a random user with license

            var fakeTrips = faker.Generate(count);

            // Seed fake trips into the database
            foreach (var tripModel in fakeTrips)
            {
                Trip trip = new Trip
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLatitude = tripModel.FromLatitude,
                    FromLongitude = tripModel.FromLongitude,
                    ToLatitude = tripModel.ToLatitude,
                    ToLongitude = tripModel.ToLongitude,
                    Date = tripModel.Date,
                    RecommendedPrice = tripModel.RecommendedPrice,
                    AvailableSeats = tripModel.AvailableSeats,
                    CarId = tripModel.CarId,
                    UserDataId = tripModel.userdataId,
                    CreatedOn = DateTime.UtcNow
                };

                await _trip.AddAsync(trip);
            }


            return Ok(fakeTrips);
        }

        [HttpPost("generate-fake-trip-Passenger")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateFakePassenger(int count)
        {
            var usercar = new List<UserData>();
            var users = await _user.GetAllAsync();
            //    var lic = await _Lic.GetAllAsync();
            foreach (var use in users)
            {
                var car = await _car.FindAsync(e => e.UserDataId == use.Id);
                if (car is null) continue;

                var x = await _user.GetByIdAsync(car.UserDataId);
                usercar.Add(use);
            }
            var nonAdminUsers = usercar.Where(u => u.Name != "Admin").ToList();

            // Generate fake trip data using Bogus
            var faker = new Faker<SeedTripPassengerModel>()
                .RuleFor(t => t.FromLatitude, f => f.Address.Latitude())
                .RuleFor(t => t.FromLongitude, f => f.Address.Longitude())
                .RuleFor(t => t.ToLatitude, f => f.Address.Latitude())
                .RuleFor(t => t.ToLongitude, f => f.Address.Longitude())
                .RuleFor(t => t.Date, f => f.Date.Future())
                .RuleFor(t => t.RecommendedPrice, f => f.Random.Float(10, 1000))
                .RuleFor(t => t.RequestedSeats, f => f.Random.Short(1, 5)) // Assuming passenger can request 1-5 seats
                .RuleFor(t => t.usrdataId, f => usercar[(f.UniqueIndex) % (nonAdminUsers.Count)].Id); // Pick a random user with license as passenger

            var fakeTrips = faker.Generate(count);

            // Seed fake trips into the database
            foreach (var tripModel in fakeTrips)
            {
                Trip trip = new Trip
                {
                    Id = Guid.NewGuid().ToString(),
                    FromLatitude = tripModel.FromLatitude,
                    FromLongitude = tripModel.FromLongitude,
                    ToLatitude = tripModel.ToLatitude,
                    ToLongitude = tripModel.ToLongitude,
                    Date = tripModel.Date,
                    RecommendedPrice = tripModel.RecommendedPrice,
                    RequestedSeats = tripModel.RequestedSeats,
                    UserDataId = tripModel.usrdataId,
                    CreatedOn = DateTime.UtcNow
                };

                await _trip.AddAsync(trip);
            }


            return Ok(fakeTrips);
        }

        #endregion

        #region Dashboard

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/TripBy/{id}")]
        public async Task<IActionResult> GetUserDataAsync([FromRoute] string id)
        {
            var result = await _service.GetTripByUserDataIdAsync(id);
            if (result is ResponseDataModel<List<GetTripByIdModel>> res)
                return Ok(new { res.data }); 

            return BadRequest(new { result.code, result.message });
        }


        [HttpGet("Admin/{id?}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTripAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetAllTripAsync();
                if (result is ResponseDataModel<List<GetTripByIdModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetByIdAsync(id);
                if (result is ResponseDataModel<List<GetTripByIdModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }


        #endregion
    }


}


