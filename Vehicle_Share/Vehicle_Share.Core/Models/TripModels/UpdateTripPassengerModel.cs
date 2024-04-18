using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class UpdateTripPassengerModel
    {
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendPrice { get; set; }
        public short RequestedSeats { get; set; } //passenger
        public bool Type { get; set; }   // Passenger or Driver

    }
}
