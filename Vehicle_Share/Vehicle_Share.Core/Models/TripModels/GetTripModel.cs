

using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripModel
    {
        public string? Id { get; set; }

        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }


        public DateTime Date { get; set; }
        public float RecommendedPrice { get; set; }
        public short? AvailableSeats { get; set; }    //driver
        public DateTime CreatedOn { get; set; }
        public int DailySchedule { get; set; }
        public int Route { get; set; }

        public short? RequestedSeats { get; set; }    //passenger
        public bool IsFinished { get; set; }
        public string? UserDataId { get; set; }
        public string? CarId { get; set; }

    }
}
