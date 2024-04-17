using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class ResetPassModel
    {
        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "CodeRequired")]
        [Range(99999, 999999, ErrorMessage = "CodeInvalid")]
        public string? Code { get; set; }
        [DataType(DataType.Password, ErrorMessage = "PasswordRequired")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirm password is not match .")]
        public string? ConfirmPassword { get; set; }
    }
}
