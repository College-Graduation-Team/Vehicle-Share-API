using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class SendCodeModel
    {
        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "PhoneInvalid")]
        public string? Phone { get; set; }
    }
}
