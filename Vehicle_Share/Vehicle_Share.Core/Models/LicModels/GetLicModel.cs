
namespace Vehicle_Share.Core.Models.LicModels
{
    public class GetLicModel
    {
        public string? Id { get; set; }
        public string? ImageFront { get; set; }
        public string? ImageBack { get; set; }
        public DateTime Expiration { get; set; }
    }
}
