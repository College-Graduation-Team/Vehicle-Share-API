﻿

namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripModel
    {
        public string? Id { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public float RecommendPrice { get; set; }
        public short? AvailableSeats { get; set; }    //driver
        public short? RequestedSeats { get; set; }    //passenger
        public bool IsFinished { get; set; }
        public string? UserDataId { get; set; }
        public string? CarId { get; set; }

    }
}
