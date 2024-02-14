using Microsoft.AspNetCore.Http;
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

		[HttpPost("Confirm-Phone")]
		public async Task<IActionResult> ConfirmedPhoneAsync(ConfirmedPhoneModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
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

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.RefreshTokenAsync(model); 
            if (!result.IsAuth)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost("Send-Code")] // for reset password and resend code 
        public async Task<IActionResult> SendCodeAsync([FromBody] SendCodeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.SendCodeAsync(model);
            if (result != "Code sent successfully")
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("Phone-Is-Confimed")]
        public async Task<IActionResult> IsPhoneConfirmedAsync([FromBody] PhoneModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _autherRepo.IsPhoneConfirmedAsync(model);
            if (!result.PhoneConfirmed)
                return BadRequest(" Phone is not Confirm  ...");
            return Ok((new { result.PhoneConfirmed, result.Message }));
        }
    }
}
