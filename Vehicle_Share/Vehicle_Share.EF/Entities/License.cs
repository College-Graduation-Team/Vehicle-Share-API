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
        public string Id { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string ImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string ImageBack { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        // relation 
        public String UserDataId { get; set; }
        [ForeignKey("UserDataId")]
        public UserData UserData { get; set; }
    }
}
