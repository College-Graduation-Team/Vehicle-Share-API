
using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.Core.Models.LicModels
{
    public class GetLicModel
    {
        public string? Id { get; set; }
        public string? UserDataId { get; set; }
        public string? ImageFront { get; set; }
        public string? ImageBack { get; set; }
        public DateTime Expiration { get; set; }

        public Status Status { get; set; }
        public string? Message { get; set; }
    }
}
