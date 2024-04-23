using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Service.CarService;
namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarController : ControllerBase
    {
        private readonly ICarServ _service;

        public CarController(ICarServ service)
        {
            _service = service;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                var result = await _service.GetAllAsync();
                if (result is ResponseDataModel<List<GetCarModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.message });
            }
            else
            {
                var result = await _service.GetByIdAsync(id);
                if (result is ResponseDataModel<GetCarModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.message });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCarAsync([FromForm] CarModel model)
        {
            if (!ModelState.IsValid || model == null)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdataCarAsync([FromRoute] string id, [FromForm] UpdateCarModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarAsync([FromRoute] string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

    }
}
