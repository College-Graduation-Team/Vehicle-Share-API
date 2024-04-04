using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Service.TripService;

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
        [HttpGet("Read-by-id/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }
        [HttpGet("Read-AllTrip")]
        public async Task<IActionResult> GetAllAsync()
        {

            var result = await _repo.GetAllForUserAsync();
            if (result != null)
                if (result.IsSuccess)
                    return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }
            [HttpGet("Read-AllTrip-Driver")]
        public async Task<IActionResult> GetAllDriverTripAsync()
        {

            var result = await _repo.GetAllDriverTripAsync();
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }
        [HttpGet("Read-AllTrip-Passenger")]
        public async Task<IActionResult> GetAllPassengerTripAsync()
        {

            var result = await _repo.GetAllPassengerTripAsync();
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpPost("Add-Trip-Driver")] // driver add trip 
        public async Task<IActionResult> AddTripAsync([FromBody] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
        [HttpPost("Add-Trip-Passenger")] //passenger add trip
        public async Task<IActionResult> AddTripAsync([FromBody] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Update-Trip-Driver/{id}")] //update for driver
        public async Task<IActionResult> UpdataTripAsync(string id, [FromForm] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
      
        [HttpPost("Update-Trip-Passenger/{id}")]//update for Passenger
        public async Task<IActionResult> UpdataTripAsync(string id, [FromForm] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
        [HttpPost("Delete-Trip/{id}")]
        public async Task<IActionResult> DeleteTripAsync([FromRoute] string id)
        {
            var result = await _repo.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

       
    }
}


