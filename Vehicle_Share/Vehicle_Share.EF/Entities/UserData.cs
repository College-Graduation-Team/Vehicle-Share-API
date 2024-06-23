
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Vehicle_Share.Core.Helper.StatusContainer;


namespace Vehicle_Share.EF.Models
{
    public class UserData
    {
        [Key]
        public string? Id { get; set; }


        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(100)]
        public string? Name { get; set; }


        [Required(ErrorMessage = "Nationail ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Nationail ID must be 14 digit .")]
        public long NationalId { get; set; }


        public DateTime Birthdate { get; set; }


        public bool Gender { get; set; }



        [Required(ErrorMessage = "Nationality is required.")]
        [MaxLength(50)]
        public string? Nationality { get; set; }



        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }


        // [Required(ErrorMessage = "Image is required.")]
        public string? NationalCardImageFront { get; set; }


        // [Required(ErrorMessage = "Image is required.")]
        public string? NationalCardImageBack { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? ProfileImage { get; set; }

        public DateTime CreatedOn { get; set; }
        public string? FcmToken { get; set; }
        public double Rating { get; set; }
        public int RatingCounter { get; set; }

        [Required]
        public Status Status { get; set; }
        public string? Message { get; set; }
        // relation 
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
