using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Vehicle_Share.EF.Helper.StatusContainer;

namespace Vehicle_Share.EF.Models
{
    public class Request
    {
        [Key]
        public string RequestID { get; set; }
        public Status RequestStatus { get; set; }

        //relation  
        // user and trip
        public string User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }

         ///Relation with Trip
        public string Trip_Id { get; set; }
        [ForeignKey("Trip_Id")]
        public Trip Trip { get; set; }
}
}
