

namespace Vehicle_Share.Core.Models.CarModels
{
    public class GetCarModel
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public int Model { get; set; }
        public string? Brand { get; set; }
        public string? Plate { get; set; }
        public int Seats { get; set; }

        public string? Image { get; set; }
        public string? LicenseImageFront { get; set; }
        public string? LicenseImageBack { get; set; }
        public DateTime LicenseExpiration { get; set; }
    }
}
