using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class Car
    {
        [Key]
        public string CarID { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Lec { get; set; }
        public string Plate { get; set; }
        public int sets { get; set; }
        public byte[] Image_Front { get; set; }
        public byte[] Image_back { get; set; }
        // relations 
        public String User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }



    }
}
