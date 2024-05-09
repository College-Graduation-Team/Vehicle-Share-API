using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.EF.Models
{
    public class Request
    {
        [Key]
        public string? Id { get; set; }
        public Status Status { get; set; }
        public short Seats { get; set; }
        public bool Type { get; set; }   // Passenger is false  or Driver is true 
        public DateTime CreatedOn { get; set; }

        //relation  
        // user and trip
        public string? UserDataId { get; set; }
        [ForeignKey("UserDataId")]
        public UserData? UserData { get; set; }

        ///Relation with Trip
        public string? TripId { get; set; }
        [ForeignKey("TripId")]
        public Trip? Trip { get; set; }
    }
}
