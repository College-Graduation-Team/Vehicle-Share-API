using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Share.EF.Models
{
    public class License
    {
        [Key]
        public string? Id { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? ImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? ImageBack { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        // relation 
        public string? UserDataId { get; set; }
        [ForeignKey("UserDataId")]
        public UserData? UserData { get; set; }
    }
}
