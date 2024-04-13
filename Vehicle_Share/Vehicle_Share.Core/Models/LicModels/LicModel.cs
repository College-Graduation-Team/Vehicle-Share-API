using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.LicModels
{
    public class LicModel
    {
        [Required(ErrorMessage = "Image is required.")]
        public IFormFile? ImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile? ImageBack { get; set; }

        [Required]
        public DateTime Expiration { get; set; }
    }
}
