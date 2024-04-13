using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Service.UserDataService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataServ _repo;

        public UserDataController(IUserDataServ repo)
        {
            _repo = repo;
        }

        [HttpGet("Read-UserData")]
        public async Task<IActionResult> GetUserDataAsync()
        {
            var result = await _repo.GetUserDataAsync();
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpPost("Add-UserData")]
        public async Task<IActionResult> AddUserDataAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

        [HttpPost("Update-UserData-{id}")]
        public async Task<IActionResult> UpdataUserDataAsync(string id,[FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id,model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }

    }
}
