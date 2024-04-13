

namespace Vehicle_Share.Core.Models.RequestModels
{
    public class GetReqModel
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? TripId { get; set; }
        public string? UserDataId { get; set; }
    }
}
