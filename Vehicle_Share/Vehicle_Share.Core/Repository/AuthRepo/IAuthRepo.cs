using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Core.Repository.AuthRepo
{
    public interface IAuthRepo
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<string> ConfirmedPhoneAsync(ConfirmPhoneModel model);
        Task<string> AddRoleAsync(RoleModel model);
        Task<AuthModel> RefreshTokenAsync(RefreshTokenModel model);
        Task<ResponseModel> SendCodeAsync(SendCodeModel model);
        Task<AuthModel> ResetPasswordAsync(ResetPassModel model);
        // Task<AuthModel> IsPhoneConfirmedAsync(PhoneModel model);
        Task<AuthModel> LogoutAsync();
        /*
        Task<string> SendMsgToConfirmAsync(EmailModel model);
         */
    }
}
