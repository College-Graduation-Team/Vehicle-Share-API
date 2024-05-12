

namespace Vehicle_Share.Core.Models.RequestModels
{
    public class GetReqModel
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? TripId { get; set; }
        public string? UserDataName { get; set; }
        public string? UserDataImage { get; set; }
    }
}
