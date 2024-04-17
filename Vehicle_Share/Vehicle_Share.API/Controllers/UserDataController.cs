using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.UserData;
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
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDataAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPut]
        public async Task<IActionResult> UpdataUserDataAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

    }
}
