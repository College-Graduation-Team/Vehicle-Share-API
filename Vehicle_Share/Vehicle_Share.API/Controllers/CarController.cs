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

        [HttpPost("Add-Car")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCarAsync([FromForm] CarModel model)
        {
            if (!ModelState.IsValid || model == null)

                return BadRequest(ModelState);


            var result = await _repo.AddAsync(model);
            if (result == "Car add successfully ")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Update-Car/{id}")]
        public async Task<IActionResult> UpdataCarAsync(string id, [FromForm] CarModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result == "Car updated successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Delete-Car/{id}")]
        public async Task<IActionResult> DeleteCarAsync(string id)
        {
            var result = await _repo.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("Read-AllCar")]
        
        public async Task<IActionResult> GetAllAsync()
        {
           

            var result = await _repo.GetAllAsync();
            if (result != null)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("Get-All-Cars")]

        public async Task<IActionResult> GetAllCarsForUser()
        {
            var result = await _repo.GetCarBrandsForUser();
            if (!result.IsNullOrEmpty())
                return Ok(result);

            return BadRequest("User or Userdata not found ! ");
        }
    }
}
