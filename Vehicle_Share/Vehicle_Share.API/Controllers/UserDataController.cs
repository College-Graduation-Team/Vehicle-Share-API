using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.UserDataService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataServ _service;

        public UserDataController(IUserDataServ service)
        {
            _service = service;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetMyUserDataAsync([FromRoute]string? id)
        {
            if (id is null)
            {
                var result = await _service.GetUserDataAsync();
                if (result is ResponseDataModel<GetUserModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserDataByIdAsyc(id);
                if (result is ResponseDataModel<GetUserModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> AddAndUpdateAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateAsync(model);
            if (result is ResponseDataModel<ProfileImageModel> res)
                return Ok(new { res.message, res.data.Id });

            return BadRequest(new { result.message });

        }
       
        [HttpPost("Image")]
        public async Task<IActionResult> AddAndUpdateNationalImageAsync([FromForm] NationalImageModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateNationalImageAsync(model);
            if (result is ResponseDataModel<ImageModel> res)
                return Ok(new { res.message, res.data.Id });

            return BadRequest(new { result.message });

        }


        #region  Admin

     

        [HttpGet("Admin/userdata/{id?}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetUserDataAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetUserDataAllAsync();
                if (result is ResponseDataModel<List<GetUserDataModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserDataByIdAsyc(id);
                if (result is ResponseDataModel<GetUserDataModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }

        [HttpGet("Admin/userdataByUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDataByUserIdAsync([FromRoute] string id)
        {
            var result = await _service.GetUserDataByUserIdAsync(id);
            if (result is ResponseDataModel<GetUserDataModel> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });

        }
            [HttpPut("Admin/UpdateDate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromForm] UserDataModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }

        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusRequestAsync([FromRoute] string id, [FromForm] UpdateStatusRequestModel model)
        {
            var result = await _service.UpdateStatusRequestAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }

        #endregion


    }
}

