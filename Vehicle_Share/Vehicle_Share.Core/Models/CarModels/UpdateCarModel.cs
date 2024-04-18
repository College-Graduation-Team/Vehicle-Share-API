using Microsoft.AspNetCore.Http;


namespace Vehicle_Share.Core.Models.CarModels
{
    public class UpdateCarModel
    {
        public string? Type { get; set; }
        public int ModelYear { get; set; }
        public string? Brand { get; set; }
        public string? Plate { get; set; }
        public short Seats { get; set; }
        public IFormFile? LicenseImageFront { get; set; }
        public IFormFile? LicenseImageBack { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime LicenseExpiration { get; set; }


    }
}
