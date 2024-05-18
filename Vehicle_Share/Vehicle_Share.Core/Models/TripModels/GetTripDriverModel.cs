namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripDriverModel
    {
        public string? Id { get; set; }

        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public DateTime Date { get; set; }
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public DateTime CreatedOn { get; set; }

        public string? CarId { get; set; } //driver
        public string? CarType { get; set; } //driver
        public string? CarBrand { get; set; } //driver

        public string? UserDataId { get; set; }


    }
}
