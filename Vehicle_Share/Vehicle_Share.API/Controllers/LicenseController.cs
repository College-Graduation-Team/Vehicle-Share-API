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
      
        [HttpGet("Read-License")]  //get from user and userdata 
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _repo.GetAsync();
            if (result.IsSuccess)
                return Ok(new { result.ErrorMesssage });

            return BadRequest(new { result.ErrorMesssage });
        }
        
        [HttpPost("Add-License")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLicenseAsync([FromForm] LicModel model)
        {
            if (!ModelState.IsValid || model == null)

                return BadRequest(ModelState);


            var result = await _repo.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Update-License-{id}")]
        public async Task<IActionResult> UpdateLicenseAsync(string id, [FromForm] LicModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Delete-License-{id}")]
        public async Task<IActionResult> DeleteLicenseAsync(string id)
        {
            var result = await _repo.DeleteAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }


    }
}

