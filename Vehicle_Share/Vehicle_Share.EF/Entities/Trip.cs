using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class Trip
    {
        [Key]
        public string TripID { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string To { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public float Recommendprice { get; set; }


        public int? AvilableSets { get; set; }    //driver
        public string? car { get; set; }    //driver
        public int? NumOfSetWant { get; set; }    //passenger


        public bool IsFinish => DateTime.UtcNow >= Date;

        // relations 
        // user and car  car can not .
        public String User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }

		public String? Car_Id { get; set; }
        [JsonIgnore]
        [ForeignKey("Car_Id")]
		public Car Car { get; set; }

        public List<Request> request { get; set; }
	}
}
