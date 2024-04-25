using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Response;
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

        [HttpGet]
        public async Task<IActionResult> GetUserDataAsync()
        {
            var result = await _service.GetUserDataAsync();
            if (result is ResponseDataModel<GetUserModel> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPost]
        public async Task<IActionResult> AddAndUpdateAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateAsync(model);
            if (result is ResponseDataModel<ImageModel> res)
                return Ok(new { res.message, res.data });

            return BadRequest(new { result.message });

        }


    }
}
