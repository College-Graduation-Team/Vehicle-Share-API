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
}
