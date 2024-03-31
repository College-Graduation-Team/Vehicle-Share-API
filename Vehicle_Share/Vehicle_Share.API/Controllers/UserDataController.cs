﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("Add-UserData")]
        public async Task<IActionResult> AddUserDataAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AddAsync(model);
            if (result == "UserData add successfully ")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Update-UserData/{id}")]
        public async Task<IActionResult> UpdataUserDataAsync(string id,[FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.UpdateAsync(id,model);
            if ( result == "User data updated successfully")
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("Read-UserData/{id}")]
        public async Task<IActionResult> GetUserDataAsync(string id)
        {
            if (id == null)
                return BadRequest("ID is not found");
          
            var result = await _repo.GetByIdAsync(id);
            if (result != null)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
