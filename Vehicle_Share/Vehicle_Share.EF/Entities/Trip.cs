using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vehicle_Share.EF.Models
{
    public class Trip
    {
        [Key]
        public string? Id { get; set; }
        [Required]//latitude and longitude
        public double FromLatitude { get; set; }
        [Required]//latitude and longitude
        public double FromLongitude { get; set; }

        [Required]
        public double ToLatitude { get; set; }
        [Required]
        public double ToLongitude { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public float RecommendedPrice { get; set; }


        public short? AvailableSeats { get; set; }    //driver
        public short? RequestedSeats { get; set; }    //passenger


        public bool IsStarted => DateTime.UtcNow >= Date;

        public bool IsFinished { get; set; }

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
