using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.IAuthService
{
    public interface IAuthServ
    {
        Task<ResponseModel> RegisterAsync(RegisterModel model);
        Task<ResponseModel> LoginAsync(LoginModel model);
        Task<ResponseModel> ConfirmedPhoneAsync(ConfirmPhoneModel model);
        Task<string> AddRoleAsync(RoleModel model);
        Task<ResponseModel> RefreshTokenAsync(RefreshTokenModel model);
        Task<ResponseModel> SendCodeAsync(SendCodeModel model);
        Task<ResponseModel> ResetPasswordAsync(ResetPassModel model);
        // Task<AuthModel> IsPhoneConfirmedAsync(PhoneModel model);
        Task<ResponseModel> LogoutAsync();
        Task<ResponseModel> DeleteAccountAsync();
        Task<bool> IsUserAdmin(string userId);
        /*
        Task<string> SendMsgToConfirmAsync(EmailModel model);
         */
    }
}
