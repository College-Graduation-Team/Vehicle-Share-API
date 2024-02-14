using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class LicenseUser
    {
        [Key]
        public string LicenseID {  get; set; }
        public DateTime StartData { get; set; }
        public DateTime EndData { get; set; }
        public byte[] Image_Front { get; set; }
        public byte[] Image_back { get; set; }

		public String User_DataId { get; set; }
		[ForeignKey("User_DataId")]
		public UserData UserData { get; set; }
	}
}
