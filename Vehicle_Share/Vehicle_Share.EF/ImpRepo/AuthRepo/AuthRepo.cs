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
using Vehicle_Share.Core.Repository.AuthRepo;
using Vehicle_Share.Core.Repository.SendOTP;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Helper;
using Vehicle_Share.EF.Models;
using static System.Net.WebRequestMethods;
namespace Vehicle_Share.EF.ImpRepo.AuthRepo
{
    public class AuthRepo : IAuthRepo
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ISendOTP _smsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public AuthRepo(UserManager<User> userManager, IOptions<JWT> jwt,
            RoleManager<IdentityRole> roleManager, ISendOTP smsService,
            IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _smsService = smsService;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
        }


        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = _LocaLizer[SharedResourcesKey.ExistsName] };

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

                return new AuthModel { Message = errors };
            }


            var otp = GenerateRandomCode();
            try
            {
                _smsService.Send(user.PhoneNumber, otp);

            }
            catch (Exception)
            {

                return new AuthModel { Message = _LocaLizer[SharedResourcesKey.Error] };
            }

            user.ResetCode = otp;
            user.ResetCodeGeneateAt = DateTime.UtcNow;

            //var roles = _roleManager.Roles.ToList();
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.UpdateAsync(user);
            return new AuthModel { Message = _LocaLizer[SharedResourcesKey.Created], IsAuth = true };
        }

        public async Task<ResponseModel> ConfirmedPhoneAsync(ConfirmPhoneModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user == null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber] };

            if (model.Code.IsNullOrEmpty() || model.Code != user.ResetCode || user.ResetCodeExpired)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongCode] };
            }

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Success], IsSuccess = true };


        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user is null ||
                !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.WrongPhoneOrPassword];
                return authModel;
            }

            if (user.PhoneNumberConfirmed == false)
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber];
                return authModel;
            }

            var jwtSecurityToken = await CreateToken(user);


            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuth = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Phone = user.PhoneNumber;
            authModel.PhoneConfirmed = user.PhoneNumberConfirmed;
            authModel.UserName = user.UserName;
            authModel.TokenExpiration = jwtSecurityToken.ValidTo.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, DateTimeFormatInfo.InvariantInfo);
            authModel.Roles = rolesList.ToList();

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
            return authModel;
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

        public async Task<AuthModel> RefreshTokenAsync(RefreshTokenModel model)
        {
            var authModel = new AuthModel();
            var user = _userManager.Users
                .Where(u => u.RefreshTokens.Any(t => t.Token == model.Token))
                .FirstOrDefault();

            if (user == null)
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.WrongToken];
                return authModel;
            }
            if (user.PhoneNumberConfirmed == false)
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber];
                return authModel;
            }
            var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == model.Token);

            if (!existingRefreshToken.IsActive)
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.WrongToken];
                return authModel;
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

            return authModel;
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
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber] };

            // Generate a random 6-digit code
            var code = GenerateRandomCode();

            user.ResetCode = code;

            user.ResetCodeGeneateAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

           _smsService.Send(user.PhoneNumber, code);

            return  new ResponseModel { message = _LocaLizer[SharedResourcesKey.Success] , IsSuccess=true };
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPassModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.Users.FirstOrDefaultAsync(o => o.PhoneNumber == model.Phone);

            if (user == null)
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber];
                return authModel;
            }

            // Check if the code has expired (assuming 5 minutes expiration)
            if (model.Code != user.ResetCode || user.ResetCodeExpired || model.Code.IsNullOrEmpty())
            {
                authModel.Message = _LocaLizer[SharedResourcesKey.WrongCode];
                return authModel;
            }

            // Check if the provided code matches the saved code
            var Token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, Token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    authModel.Message = $"Error: {error.Code}, Description: {error.Description}";
                    return authModel;
                }
            }

            // Clear the reset code after successful password reset
            user.ResetCode = null;
            await _userManager.UpdateAsync(user);

            authModel.Message = "Password reset successfully";
            authModel.IsAuth = true;
            return authModel;
        }

        // public async Task<AuthModel> IsPhoneConfirmedAsync(PhoneModel model)
        // {
        //     var user = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.Phone);
        //     if (user == null)
        //     {
        //         return new AuthModel { Message = _LocaLizer[SharedResourcesKey.WrongPhoneNumber] };
        //     }
        //     // Decode the JWT token to get the user's phone
        //     string userPhoneFromToken = DecodeJwtToken(model.Token);

        //     // Check if the decoded phone matches the user's phone
        //     if (userPhoneFromToken != user.PhoneNumber)
        //     {
        //         return new AuthModel { Message = _LocaLizer[SharedResourcesKey.WrongToken] };
        //     }


        //     return user.PhoneNumberConfirmed
        //         ? new AuthModel { Message = _LocaLizer[SharedResourcesKey.ConfirmPhoneNumber], PhoneConfirmed = true }
        //         : new AuthModel { Message = _LocaLizer[SharedResourcesKey.NotConfirmPhoneNumber], PhoneConfirmed = false };
        // }

        public async Task<AuthModel> LogoutAsync()
        {
            // Get the current user's ID from HttpContext
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId == null)
            {
                return new AuthModel { Message = "User not Authorize" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthModel { Message = "User not found" };
            }
            var authModel = new AuthModel();

            user.RefreshTokens.Clear();

            authModel.IsAuth = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new AuthModel { Message = " Logout failed " };
            authModel.Message = " Logout successful  ";
            return authModel;
        }
        //  private method to create GenerateRandomCode
        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
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

        // private string DecodeJwtToken(string jwtToken)
        // {
        //     var handler = new JwtSecurityTokenHandler();

        //     if (handler.CanReadToken(jwtToken))
        //     {
        //         try
        //         {
        //             var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

        //             if (jsonToken != null)
        //             {
        //                 // Check if the token is expired
        //                 if (jsonToken.ValidTo < DateTime.UtcNow)
        //                 {
        //                     // Token is expired
        //                     return "Token is expired ";
        //                 }

        //                 // Extract the user email from the token
        //                 var PhoneNumber = jsonToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
        //                 return PhoneNumber;
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             return "decoding errors or invalid tokens";
        //             throw;
        //         }

        //     }
        //     _userManager.FindByIdAsync(jwtToken).Wait();

        //     // Handle decoding errors or invalid tokens
        //     return "decoding errors or invalid tokens";
        // }
    }
}
