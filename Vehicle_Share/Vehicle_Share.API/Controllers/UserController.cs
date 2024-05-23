using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Vehicle_Share.Core.Models.UserData;

using Vehicle_Share.Core.Response;
using Vehicle_Share.Service.UserDataService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class UserController : ControllerBase
    {
        private readonly IUserDataServ _service;
        public UserController(IUserDataServ service)
        {
            _service = service;
        }
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetAllUserAsync();
                if (result is ResponseDataModel<List<GetAllUsersModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserByIdAsyc(id);
                if (result is ResponseDataModel<GetAllUsersModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }

    }
}
