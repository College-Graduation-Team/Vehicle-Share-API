

namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripModel
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendPrice { get; set; }
        public int? AvailableSeats { get; set; }    //driver
        public int? RequestedSeats { get; set; }    //passenger
        public bool IsFinished { get; set; }
        public String UserDataId { get; set; }
        public String? CarId { get; set; }
    
    }
}
