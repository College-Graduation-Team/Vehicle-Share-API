using Microsoft.AspNetCore.Http;

namespace Vehicle_Share.Core.Models.UserData
{
    public class NationalImageModel
    {
        public IFormFile? NationalCardImageFront { get; set; }


        // [Required(ErrorMessage = "Image is required.")]
        public IFormFile? NationalCardImageBack { get; set; }
    }
}
