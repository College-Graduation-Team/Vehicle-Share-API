using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Service.LicenseService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {

        private readonly ILicServ _repo;

        public LicenseController(ILicServ repo)
        {
            _repo = repo;
        }

        [HttpPost("Add-License")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLicenseAsync([FromForm] LicModel model)
        {
            if (!ModelState.IsValid || model == null)

                return BadRequest(ModelState);


            var result = await _repo.AddAsync(model);
            if (result == "License added successfully ")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Update-License/{id}")]
        public async Task<IActionResult> UpdateLicenseAsync(string id, [FromForm] LicModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result == "License updated successfully")
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("Read-License")]
        //get from user and userdata 
        public async Task<IActionResult> GetAllAsync()
        {
           

            var result = await _repo.GetAllAsync();
            if (result != null)
                return Ok(result);

            return BadRequest("User or Userdata not found ! ");
        }
    }
}

