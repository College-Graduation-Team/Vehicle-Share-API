using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class LoginModel
    {

        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string? Phone { get; set; }


        [DataType(DataType.Password,ErrorMessage ="PasswordRequired")]
        public string? Password { get; set; }
    }
}
