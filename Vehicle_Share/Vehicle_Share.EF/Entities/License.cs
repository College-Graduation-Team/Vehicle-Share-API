using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class License
    {
        [Key]
        public string LicID { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string LicUserImgFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string LicUserImgBack { get; set; }

        [Required]
        public DateTime EndDataOfUserLic { get; set; }

        // relation 
        public String User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }
    }
}
