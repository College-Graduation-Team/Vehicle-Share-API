
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vehicle_Share.EF.Models
{
    public class Car
    {
        [Key]
        public string? Id { get; set; }


        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(30, ErrorMessage = "the max length is 30 char")]
        public string? Type { get; set; }


        [Required(ErrorMessage = "Model is required.")]
        [MaxLength(4, ErrorMessage = "the max length is 4 digit")]
        [StringLength(4, ErrorMessage = "the max length is 4 digit", MinimumLength = 4)]
        public int ModelYear { get; set; }


        [Required(ErrorMessage = "Brand is required.")]
        [MaxLength(30, ErrorMessage = "the max length is 30 char")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [MaxLength(15, ErrorMessage = "the max length is 15 char")]
        public string? Plate { get; set; }


        [Required(ErrorMessage = "SetsOfCar is required.")]
        [MaxLength(2, ErrorMessage = "the max length is 2 digit")]
        public short Seats { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? Image { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? LicenseImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string? LicenseImagBack { get; set; }
        [Required]
        public DateTime LicenseExpiration { get; set; }
        public DateTime CreatedOn { get; set; }


        // relations 
        public string? UserDataId { get; set; }
        [ForeignKey("UserDataId")]
        public UserData? UserData { get; set; }


    }
}
