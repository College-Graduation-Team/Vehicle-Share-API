using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "UserName")]
        public string? UserName { get; set; }


        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string? Phone { get; set; }


        [DataType(DataType.Password, ErrorMessage = "PasswordRequired")]
        public string? Password { get; set; }


        [Compare("Password", ErrorMessage = "The password and confirm password is not match.")]
        public string? ConfirmPassword { get; set; }

    }
}
