using Bogus;
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

        public TripController(ITripServ service)
        {
            _service = service;
          
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


        [HttpGet("userdata-ids/{tripId}")]
        public async Task<IActionResult> GetAllUserDataIdsInTripAsync([FromRoute]string tripId)
        {
            var result = await _service.GetAllUserDataIdsInTripAsync(tripId);

            if (result is ResponseDataModel<UserIdsModel> res)
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

        [HttpPut("finish/{id}")] //update for driver
        public async Task<IActionResult> TripFinished([FromRoute] string id)
        {

            var result = await _service.TripFinishedAsync(id);
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

       

        #region Dashboard

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin/TripById/{id}")]
        public async Task<IActionResult> GetTripByUserDataIdAsync([FromRoute] string id)
        {
            var result = await _service.GetTripByUserDataIdAsync(id);
            if (result is ResponseDataModel<GetTripByIdModel> res)
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
                if (result is ResponseDataModel<List<GetTripModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetByIdAsync(id);
                if (result is ResponseDataModel<GetTripByIdModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }


        #endregion
    }


}


