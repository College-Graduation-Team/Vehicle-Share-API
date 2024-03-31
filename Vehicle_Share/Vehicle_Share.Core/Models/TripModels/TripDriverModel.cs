using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class TripDriverModel
    {
        [Required]
        [MaxLength(50,ErrorMessage ="the length is 50 char ")]
        public string From { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "the length is 50 char ")]
        public string To { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public float Recommendprice { get; set; }
        public int AvilableSets { get; set; } //driver
        public string CarID { get; set; } //driver
    }
}
