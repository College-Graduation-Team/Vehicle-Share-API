using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Service.TripService;
using Vehicle_Share.Service.UserDataService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripController : ControllerBase
    {
        private readonly ITripServ _repo;

        public TripController(ITripServ repo)
        {
            _repo = repo;
        }

        [HttpGet("Read-AllTrip")]
        public async Task<IActionResult> GetAllAsync()
        {

            var result = await _repo.GetAllForUserAsync();
            if (result != null)
                return Ok(result);

            return BadRequest("User or Userdata not found ! ");
        }
        [HttpGet("Read-AllTrip-Passenger")]
        public async Task<IActionResult> GetAllAsDriverAsync()
        {

            var result = await _repo.GetAllAsDriverAsync();
            if (result != null)
                return Ok(result);

            return BadRequest("User or Userdata not found ! ");
        }
        [HttpGet("Read-AllTrip-Driver")]
        public async Task<IActionResult> GetAllAsPassengerAsync()
        {

            var result = await _repo.GetAllAsPassengerAsync();
            if (result != null)
                return Ok(result);

            return BadRequest("User or Userdata not found ! ");
        }

        [HttpPost("Add-Trip-Driver")] // driver add trip 
        public async Task<IActionResult> AddTripAsync([FromBody] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result == " Trip add successfully ")
                return Ok(result);

            return BadRequest(result);
        }
        [HttpPost("Add-Trip-Passenger")] //passenger add trip
        public async Task<IActionResult> AddTripAsync([FromBody] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result == " Trip add successfully ")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Update-Trip-Driver/{id}")] //update for driver
        public async Task<IActionResult> UpdataTripAsync(string id, [FromForm] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result == "Trip updated successfully")
                return Ok(result);

            return BadRequest(result);
        }
      
        [HttpPost("Update-Trip-Passenger/{id}")]//update for Passenger
        public async Task<IActionResult> UpdataTripAsync(string id, [FromForm] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result == "Trip updated successfully")
                return Ok(result);

            return BadRequest(result);
        }
        [HttpPost("Delete-Trip/{id}")]
        public async Task<IActionResult> DeleteTripAsync(string id)
        {
            var result = await _repo.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

       
    }
}


