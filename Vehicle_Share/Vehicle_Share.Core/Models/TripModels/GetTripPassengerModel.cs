namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripPassengerModel
    {
        public string? Id { get; set; }
        
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public DateTime Date { get; set; }
        public float RecommendedPrice { get; set; }
        public DateTime CreatedOn { get; set; }

        public short RequestedSeats { get; set; } //passenger
        public bool IsFinished { get; set; }
    }
}
