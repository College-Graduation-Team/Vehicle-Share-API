using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Service.LicenseService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LicenseController : ControllerBase
    {

        private readonly ILicServ _service;

        public LicenseController(ILicServ service)
        {
            _service = service;
        }

        [HttpGet]  //get from user and userdata 
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _service.GetLicenseAsync();
            if (result is ResponseDataModel<GetLicModel> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLicenseAsync([FromForm] LicModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateAsync(model);
            if (result is ResponseDataModel<GetImageModel> res)
                return Ok(new { res.message, res.data });

            return BadRequest(new { result.message });
        }


        [HttpDelete()]
        public async Task<IActionResult> DeleteLicenseAsync()
        {
            var result = await _service.DeleteAsync();
            if (result .IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

    }
}

