using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class PhoneModel
    {

        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "TokenRequired")]
        public string? Token { get; set; }
    }
}
