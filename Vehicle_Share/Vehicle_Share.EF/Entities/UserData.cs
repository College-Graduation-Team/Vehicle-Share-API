
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vehicle_Share.EF.Models
{
    public class UserData
    {
        [Key]
        public string Id { get; set; }


        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(100)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Nationail ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Nationail ID must be 14 digit .")]
        public long NationailId { get; set; }


        public DateTime Birthdata { get; set; }


        public  bool Gender { get; set; }



        [Required(ErrorMessage = "Nationality is required.")]
        [MaxLength(50)]
        public  string Nationality { get; set; }



        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string NationalCardImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string NationalCardImageBack { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string ProfileImage { get; set; }

        public bool Type { get; set; }    // driver of passenger
        // relation 
        public String UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
