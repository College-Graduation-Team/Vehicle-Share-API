using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class UpdateTripDriverModel
    {
        
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendedPrice { get; set; }
        public short AvailableSeats { get; set; } //driver
        public string? CarId { get; set; } //driver


    }
}
