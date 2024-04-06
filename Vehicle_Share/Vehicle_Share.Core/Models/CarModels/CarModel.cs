using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Vehicle_Share.Core.Models.CarModels
{
    public class CarModel
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public int ModelYear { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Plate { get; set; }
        [Required]
        public short Seats { get; set; }
        [Required]
        public IFormFile LicenseImageFront { get; set; }
        [Required]
        public IFormFile LicenseImageBack { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        public DateTime LicenseExpiration { get; set; }


    }
}
