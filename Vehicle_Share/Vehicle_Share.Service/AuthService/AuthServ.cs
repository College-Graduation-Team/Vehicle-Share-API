using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Service.IAuthService;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Helper;
using Vehicle_Share.EF.Models;
using Twilio.TwiML.Messaging;
using Vehicle_Share.EF.ImpRepo.SendOTPImplement;
namespace Vehicle_Share.Service.AuthService
{
    public class AuthServ : IAuthServ
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public AuthServ(UserManager<User> userManager, IOptions<JWT> jwt,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
        }


        public async Task<ResponseModel> RegisterAsync(RegisterModel model)
        {

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.ExistsName], code = ResponseCode.ExistsUser };

            var user = new User
            {
                UserName = model.UserName,
                PhoneNumber = model.Phone,
                CreatedOn = DateTime.UtcNow,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new ResponseModel { message = errors, code = ResponseCode.InValidPassword };
            }


            var otp = GenerateRandomCode();

            await SendOTP.Send(user.PhoneNumber, otp);

            user.ResetCode = otp;
            user.ResetCodeGeneateAt = DateTime.UtcNow;

            //var roles = _roleManager.Roles.ToList();
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.UpdateAsync(user);
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }

        public async Task<ResponseModel> ConfirmedPhoneAsync(ConfirmPhoneModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber]};

            if (model.Code.IsNullOrEmpty() || model.Code != user.ResetCode || user.ResetCodeExpired)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongCode]};
            }

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Success], IsSuccess = true };


        }

        public async Task<ResponseModel> LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user is null ||
                !await _userManager.CheckPasswordAsync(user, model.Password))
            {
               // authModel.Message = _LocaLizer[SharedResourcesKey.WrongPhoneOrPassword];
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneOrPassword] ,code=ResponseCode.WrongPhoneOrPassword };
            }

            if (user.PhoneNumberConfirmed == false)
            {
                // authModel.Message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber];
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber], code=ResponseCode.NotConfirmPhoneNumber };
            }

            var jwtSecurityToken = await CreateToken(user);


            var rolesList = await _userManager.GetRolesAsync(user);

           //  authModel.IsAuth = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
           // authModel.Phone = user.PhoneNumber;
            authModel.PhoneConfirmed = user.PhoneNumberConfirmed;
           // authModel.UserName = user.UserName;
            authModel.TokenExpiration = jwtSecurityToken.ValidTo.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo);
            //authModel.Roles = rolesList.ToList();

            await _userManager.UpdateAsync(user);
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo); ;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return new ResponseDataModel<AuthModel> { data = authModel , IsSuccess=true };
        }

        public async Task<JwtSecurityToken> CreateToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, user.PhoneNumber),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<ResponseModel> RefreshTokenAsync(RefreshTokenModel model)
        {
            var authModel = new AuthModel();
            var user = _userManager.Users
                .Where(u => u.RefreshTokens.Any(t => t.Token == model.Token))
                .FirstOrDefault();

            if (user == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongToken]};
            }
            if (user.PhoneNumberConfirmed == false)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber] ,code=ResponseCode.NotConfirmPhoneNumber };
            }
            var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == model.Token);

            if (!existingRefreshToken.IsActive)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongToken] };
            }

            // Revoke the existing refresh token
            existingRefreshToken.RevokedOn = DateTime.UtcNow;

            // Generate a new refresh token
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            // Update the user in the database
            await _userManager.UpdateAsync(user);

            // Create a new JWT token
            var jwtToken = await CreateToken(user);

            authModel.IsAuth = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.TokenExpiration = jwtToken.ValidTo.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo);
            authModel.Phone = user.PhoneNumber;
            authModel.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo); ;

            return new ResponseDataModel<AuthModel> { data = authModel, IsSuccess = true };
        }

        // Add role to user 
        public async Task<string> AddRoleAsync(RoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.RoleName))
                return "Invalid user ID or role ";
            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "role is Exist !";
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result.Succeeded ? "Add role success !" : "something want Worng";

        }

        // reset password
        public async Task<ResponseModel> SendCodeAsync(SendCodeModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber] , code=0 };

            // Generate a random 6-digit code
            var code = GenerateRandomCode();

            user.ResetCode = code;

            user.ResetCodeGeneateAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

           await SendOTP.Send(user.PhoneNumber, code);

            return  new ResponseModel { message = _LocaLizer[SharedResourcesKey.Success] , IsSuccess=true };
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPassModel model)
        {
            //var authModel = new AuthModel();
            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber] ,code=0 };
              
            }

            // Check if the code has expired (assuming 5 minutes expiration)
            if (model.Code != user.ResetCode || user.ResetCodeExpired || model.Code.IsNullOrEmpty())
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongCode] ,code=1 };
            }

            // Check if the provided code matches the saved code
            var Token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, Token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return new ResponseModel { message = $"Error: {error.Code}, Description: {error.Description}" };
                }
            }

            // Clear the reset code after successful password reset
            user.ResetCode = null;
            await _userManager.UpdateAsync(user);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.ResetPassword], IsSuccess = true };
        }
        
        public async Task<ResponseModel> DeleteAccountAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId == null)
            {
                return new ResponseModel { message = "User not Authorize" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseModel { message = "User not found" };
            }
            // Handle any associated data cleanup here (e.g., user profile, settings, etc.)

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new ResponseModel { message = errors };
            }

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Deleted] , IsSuccess=true};
        }
        
        public async Task<ResponseModel> LogoutAsync()
        {
            // Get the current user's ID from HttpContext
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUser] };
            }
            var authModel = new AuthModel();

            user.RefreshTokens.Clear();

            authModel.IsAuth = false;

            await _userManager.UpdateAsync(user);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Logout], IsSuccess = true };
        }
        //  private method to create GenerateRandomCode
        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        public async Task<bool> IsUserAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "admin");
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNum = new byte[32];
            using var genertor = new RNGCryptoServiceProvider();
            genertor.GetBytes(randomNum);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNum),
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                CreatedOn = DateTime.UtcNow
            };
        }

      
    }
}
