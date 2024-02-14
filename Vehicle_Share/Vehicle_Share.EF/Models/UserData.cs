using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class UserData
    {
        [Key]
        public string UserDataID { get; set; }
        public string FullName { get; set; }
        public int NationailID { get; set; }
        public DateTime BirthData { get; set; }
        public  string Gender { get; set; }
        public  string Nationality { get; set; }
        public  string Address { get; set; }
        public  string Email { get; set; }
        public byte[] Image_Front { get; set; }
        public byte[] Image_back { get; set; }
        public string nationalCard { get; set; }

        // relation 
        public String User_Id { get; set; }
        [ForeignKey("User_Id")]
        public User User { get; set; }
    }
}
