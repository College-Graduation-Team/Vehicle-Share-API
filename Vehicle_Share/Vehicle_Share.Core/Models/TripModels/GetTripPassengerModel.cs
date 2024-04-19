namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripPassengerModel
    {
        public string? Id { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendedPrice { get; set; }
        public DateTime CreatedOn { get; set; }

        public short RequestedSeats { get; set; } //passenger
        public bool IsFinished { get; set; }
    }
}
