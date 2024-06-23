using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripPassengerModel
    {
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }


        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short RequestedSeats { get; set; } //passenger

        public int DailySchedule { get; set; }
        public int Route { get; set; }

    }
    public class SeedTripPassengerModel
    {
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }



        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float RecommendedPrice { get; set; }
        public short RequestedSeats { get; set; } //passenger
        public string usrdataId { get; set; } //passenger
    }
}
