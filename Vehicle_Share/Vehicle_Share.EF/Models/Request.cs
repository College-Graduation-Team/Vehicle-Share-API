using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class Request
    {
        [Key]
        public string RequestID { get; set; }
        public bool IsAccept { get; set; }

        //relation  
        // user and trip
        // [ForeignKey] and [InverseProperty] helps to explicitly define the relationship and avoid conflicts .
        public string User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }

        //// Relation with Trip
        //public string Trip_Id { get; set; }
        //[ForeignKey("Trip_Id")]
        //public Trip Trip { get; set; }
    }
}
