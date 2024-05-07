﻿using Bogus;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.IAuthService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthServ _autherRepo;

        public AccountController(IAuthServ autherServ)
        {
            _autherRepo = autherServ;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.RegisterAsync(model);
            if (!result.IsSuccess)
                return BadRequest(new {result.code, result.message });

            return Ok(new { result.message });
        }

        [HttpPost("confirm-phone")]
        public async Task<IActionResult> ConfirmedPhoneAsync(ConfirmPhoneModel model)
        {
            var result = await _autherRepo.ConfirmedPhoneAsync(model);
            if (!result.IsSuccess)
                return BadRequest(new { result.message });
            return Ok(new { result.message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.LoginAsync(model);

            if (result is ResponseDataModel<AuthModel> res)
                return Ok(new { res.data.Token, res.data.TokenExpiration, res.data.RefreshToken, res.data.RefreshTokenExpiration });
            return BadRequest(new { result.code, result.message });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.RefreshTokenAsync(model);
            if (result is ResponseDataModel<AuthModel> res)
                return Ok(new { res.data.Token, res.data.TokenExpiration, res.data.RefreshToken, res.data.RefreshTokenExpiration });
            return BadRequest(new { result.code, result.message });
        }

        [HttpPost("send-code")] //  resend code 
        public async Task<IActionResult> SendCodeAsync([FromBody] SendCodeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.SendCodeAsync(model);
            if (!result.IsSuccess)
                return BadRequest(new { result.message });
            return Ok(new {result.message});
        }

        [HttpPost("reset-password")] // for reset password 
        public async Task<IActionResult> ResetPasswordAsync(ResetPassModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.ResetPasswordAsync(model);
            if (!result.IsSuccess)
                return BadRequest(new { result.message ,result.code });
            return Ok(new { result.message });

        }

        [HttpDelete] // for reset password 
        public async Task<IActionResult> DeleteAccountAsync()
        {
           
            var result = await _autherRepo.DeleteAccountAsync();
            if (!result.IsSuccess)
                return BadRequest(new { result.message});
            return Ok(new { result.message });

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            var result = await _autherRepo.LogoutAsync();
            if (result.IsSuccess)
                return Ok(new {result.message});
            return BadRequest(new { result.message });
        }

        [HttpPost("generate-fake-users")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GenerateFakeUsers(int count)
        {
            var faker = new Faker<RegisterModel>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Phone, f => f.Random.Replace("+2012345678##"))
                .RuleFor(u => u.Password, f => "@Abdo123")
                .RuleFor(u => u.ConfirmPassword, f => "@Abdo123");

            var fakeUsers = faker.Generate(count);

            // Save fake users to the database or use them as needed
            foreach (var user in fakeUsers)
            {
                await _autherRepo.RegisterAsync(user);
            }

            return Ok(fakeUsers);
        }
  
    }
}
