using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.LicModels
{
    public class LicModel
    {
        public IFormFile? ImageFront { get; set; }


        public IFormFile? ImageBack { get; set; }

        public DateTime Expiration { get; set; }
    }

    public class LicSeedModel
    {
        public string? ImageFront { get; set; }


        public string? ImageBack { get; set; }

        public DateTime Expiration { get; set; }
        public string UserDataId { get; set; }
    }
}
