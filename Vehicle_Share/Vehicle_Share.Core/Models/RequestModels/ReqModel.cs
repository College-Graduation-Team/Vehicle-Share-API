﻿

namespace Vehicle_Share.Core.Models.RequestModels
{
    public class ReqModel
    {
        public short Seats { get; set; }
        public string? TripId { get; set; }
        public bool Type { get; set; }   // Passenger or Driver

    }
}
