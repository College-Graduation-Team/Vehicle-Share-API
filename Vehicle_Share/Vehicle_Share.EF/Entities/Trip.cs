using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vehicle_Share.EF.Models
{
    public class Trip
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string? To { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public float RecommendPrice { get; set; }


        public short? AvailableSeats { get; set; }    //driver
        public short? RequestedSeats { get; set; }    //passenger


        public bool IsFinished => DateTime.UtcNow >= Date;

        public DateTime CreatedOn { get; set; }


        // relations 
        // user and car  car can not .
        public string? UserDataId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserDataId")]
        public UserData? UserData { get; set; }

        public string? CarId { get; set; } = null;
        [JsonIgnore]
        [ForeignKey("CarId")]
        public Car? Car { get; set; }
    }
}
