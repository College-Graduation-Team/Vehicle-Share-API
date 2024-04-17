using System.ComponentModel.DataAnnotations;


namespace Vehicle_Share.Core.Models.AuthModels
{
    public class ConfirmPhoneModel
    {

        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string Phone { get; set; } = string.Empty;


        [Required(ErrorMessage = "CodeRequired")]
        [Range(99999, 999999, ErrorMessage = "CodeInvalid")]
        public string Code { get; set; } = string.Empty;

    }
}
