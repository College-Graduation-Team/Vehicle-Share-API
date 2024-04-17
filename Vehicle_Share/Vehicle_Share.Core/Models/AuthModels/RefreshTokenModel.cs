using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessage = "TokenRequired")]
        public string? Token { get; set; }
    }
}
