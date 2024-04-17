using Microsoft.AspNetCore.Authorization;
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
        private readonly ITripServ _service;

        public TripController(ITripServ service)
        {
            _service = service;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string? id)
        {
            if (id == null) {
                var result = await _service.GetAllForUserAsync();
                if (result.IsSuccess)
                    return Ok(new { result.data });

                return BadRequest(new { result.message });
            } else {
                var result = await _service.GetByIdAsync(id);
                if (result.IsSuccess)
                    return Ok(new { result.data });

                return BadRequest(new { result.message });
            }
        }
        
        [HttpGet("driver")]
        public async Task<IActionResult> GetAllDriverTripAsync()
        {
            var result = await _service.GetAllDriverTripAsync();
            if (result.IsSuccess)
                return Ok(new { result.data });

            return BadRequest(new { result.message });
        }

        [HttpGet("passenger")]
        public async Task<IActionResult> GetAllPassengerTripAsync()
        {

            var result = await _service.GetAllPassengerTripAsync();
            if (result.IsSuccess)
                return Ok(new { result.data });

            return BadRequest(new { result.message });
        }

        [HttpPost("driver")] // driver add trip 
        public async Task<IActionResult> AddTripAsync([FromBody] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPost("passenger")] //passenger add trip
        public async Task<IActionResult> AddTripAsync([FromBody] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPut("driver/{id}")] //update for driver
        public async Task<IActionResult> UpdataTripAsync([FromRoute] string id, [FromForm] TripDriverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPut("passenger/{id}")]//update for Passenger
        public async Task<IActionResult> UpdataTripAsync([FromRoute] string id, [FromForm] TripPassengerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTripAsync([FromRoute] string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

    }
}


