using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
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


        #region Admin

        [HttpGet("Admin/{id?}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLicenseByUserDataAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetAllAsync();
                if (result is ResponseDataModel<List<GetLicModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserDataByIdAsyc(id);
                if (result is ResponseDataModel<GetLicModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }

        [HttpPut("Admin/UpdateDate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromForm] LicModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }


        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusRequestAsync([FromRoute] string id, [FromBody] UpdateStatusRequestModel model)
        {
            var result = await _service.UpdateStatusRequestAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }


        #endregion

       


    }
}

