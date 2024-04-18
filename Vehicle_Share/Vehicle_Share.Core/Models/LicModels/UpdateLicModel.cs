using Microsoft.AspNetCore.Http;

namespace Vehicle_Share.Core.Models.LicModels
{
    public class UpdateLicModel
    {
        public IFormFile? ImageFront { get; set; }

        public IFormFile? ImageBack { get; set; }

        public DateTime Expiration { get; set; }
    }
}
