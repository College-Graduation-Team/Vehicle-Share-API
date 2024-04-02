using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Service.CarService;
namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarController : ControllerBase
    {
        private readonly ICarServ _repo;

        public CarController(ICarServ repo)
        {
            _repo = repo;
        }

        [HttpGet("Read-by-id")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new {result.ErrorMesssage});
        }

        [HttpGet("Read-All-Cars")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _repo.GetAllAsync();
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpPost("Add-Car")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCarAsync([FromForm] CarModel model)
        {
            if (!ModelState.IsValid || model == null)

                return BadRequest(ModelState);


            var result = await _repo.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new {result.Messsage});

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Update-Car/{id}")]
        public async Task<IActionResult> UpdataCarAsync(string id, [FromForm] CarModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Delete-Car/{id}")]
        public async Task<IActionResult> DeleteCarAsync(string id)
        {
            var result = await _repo.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }


    }
}
