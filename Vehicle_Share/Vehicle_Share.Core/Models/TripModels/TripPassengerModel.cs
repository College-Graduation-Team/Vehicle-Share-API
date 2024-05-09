using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripPassengerModel
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
        public short RequestedSeats { get; set; } //passenger
    }
    public class SeedTripPassengerModel
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
        public short RequestedSeats { get; set; } //passenger
        public string usrdataId { get; set; } //passenger
    }
}
