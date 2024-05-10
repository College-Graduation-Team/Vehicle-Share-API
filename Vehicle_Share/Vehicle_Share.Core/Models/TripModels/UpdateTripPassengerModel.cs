using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class UpdateTripPassengerModel
    {
        public double? FromLatitude { get; set; }
        public double? FromLongitude { get; set; }

        public double? ToLatitude { get; set; }
        public double? ToLongitude { get; set; }


        public string? Date { get; set; }
        public float? RecommendedPrice { get; set; }
        public short? RequestedSeats { get; set; } //passenger

    }
}
