

using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.Core.Models.CarModels
{
    public class GetCarModel
    {
        public string? Id { get; set; }
        public string? UserDataId { get; set; }
        public string? Type { get; set; }
        public int ModelYear { get; set; }
        public string? Brand { get; set; }
        public string? Plate { get; set; }
        public short Seats { get; set; }

        public string? Image { get; set; }
        public string? LicenseImageFront { get; set; }
        public string? LicenseImageBack { get; set; }
        public DateTime LicenseExpiration { get; set; }

        public Status Status { get; set; }
        public string? Message { get; set; }
    }
}
