using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripDriverModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver

    }
    public class SeedTripDriverModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver
        public string? userdataId { get; set; } //driver

    }
}
