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
        public string Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string To { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public float RecommendPrice { get; set; }


        public int? AvailableSeats { get; set; }    //driver
        public int? RequestedSeats { get; set; }    //passenger


        public bool IsFinished => DateTime.UtcNow >= Date;

        // relations 
        // user and car  car can not .
        public String UserDataId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserDataId")]
        public UserData UserData { get; set; }

        public String? CarId { get; set; } = null;
        [JsonIgnore]
        [ForeignKey("CarId")]
		public Car? Car { get; set; }
	}
}
