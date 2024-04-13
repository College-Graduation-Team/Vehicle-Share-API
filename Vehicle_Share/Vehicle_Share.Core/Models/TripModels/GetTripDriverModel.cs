namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripDriverModel
    {
        public string? Id { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendPrice { get; set; }
        public int AvailableSeats { get; set; } //driver

        public string? CarId { get; set; } //driver
        public string? CarType { get; set; } //driver
        public string? CarBrand { get; set; } //driver
    }
}
