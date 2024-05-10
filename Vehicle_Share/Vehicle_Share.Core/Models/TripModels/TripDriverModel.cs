using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripDriverModel
    {
        [Required]//latitude and longitude
        public double FromLatitude { get; set; }
        [Required]//latitude and longitude
        public double FromLongitude { get; set; }

        [Required]
        public double ToLatitude { get; set; }
        [Required]
        public double ToLongitude { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver

    }
    public class SeedTripDriverModel
    {
        [Required]//latitude and longitude
        public double FromLatitude { get; set; }
        [Required]//latitude and longitude
        public double FromLongitude { get; set; }

        [Required]
        public double ToLatitude { get; set; }
        [Required]
        public double ToLongitude { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver
        public string? userdataId { get; set; } //driver

    }
}
