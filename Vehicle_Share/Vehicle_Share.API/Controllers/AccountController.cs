using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Repository.AuthRepo;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthRepo _autherRepo;

        public AccountController(IAuthRepo autherServ)
        {
            _autherRepo = autherServ;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.RegisterAsync(model);
            if (!result.IsAuth)
                return BadRequest(result.Message);

            return Ok(new { result.Message });
        }

        [HttpPost("confirm-phone")]
        public async Task<IActionResult> ConfirmedPhoneAsync(ConfirmPhoneModel model)
        {
            var result = await _autherRepo.ConfirmedPhoneAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.LoginAsync(model);
            if (!result.IsAuth)
                return BadRequest(result.Message);
            return Ok(new { result.Token, result.TokenExpiration, result.RefreshToken, result.RefreshTokenExpiration, result.PhoneConfirmed });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.RefreshTokenAsync(model);
            if (!result.IsAuth)
                return BadRequest(result.Message);
            return Ok(new { result.Token, result.TokenExpiration, result.RefreshToken, result.RefreshTokenExpiration, result.PhoneConfirmed });
        }

        [HttpPost("send-code")] //  resend code 
        public async Task<IActionResult> SendCodeAsync([FromBody] SendCodeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.SendCodeAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(new {result.message});
        }

        [HttpPost("reset-password")] // for reset password 
        public async Task<IActionResult> ResetPasswordAsync(ResetPassModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.ResetPasswordAsync(model);
            if (!result.IsAuth)
                return BadRequest(result.Message);
            return Ok(result.Message);

        }
        
        // [HttpPost("is-phone-confimed")]
        // public async Task<IActionResult> IsPhoneConfirmedAsync([FromBody] PhoneModel model)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);
        //     var result = await _autherRepo.IsPhoneConfirmedAsync(model);
        //     if (!result.PhoneConfirmed)
        //         return BadRequest("Phone isn't Confirmed");
        //     return Ok(new { result.PhoneConfirmed, result.Message });
        // }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            var result = await _autherRepo.LogoutAsync();
            if (result.IsAuth)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
