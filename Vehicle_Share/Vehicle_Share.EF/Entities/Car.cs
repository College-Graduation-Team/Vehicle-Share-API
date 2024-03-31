
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vehicle_Share.EF.Models
{
    public class Car
    {
        [Key]
        public string CarID { get; set; }


        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(30,ErrorMessage ="the max length is 30 char")]
        public string Type { get; set; }


        [Required(ErrorMessage = "Model is required.")]
        [MaxLength(4, ErrorMessage = "the max length is 4 digit")]
        [StringLength(4, ErrorMessage = "the max length is 4 digit",MinimumLength =4)]
        public int Model { get; set; }


        [Required(ErrorMessage = "Brand is required.")]
        [MaxLength(30, ErrorMessage = "the max length is 30 char")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [MaxLength(15, ErrorMessage = "the max length is 15 char")]
        public string CarPlate { get; set; }


        [Required(ErrorMessage = "SetsOfCar is required.")]
        [MaxLength(2, ErrorMessage = "the max length is 2 digit")]
        public int SetsOfCar { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string CarImg { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string LicCarImgFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string LicCarImgBack { get; set; }
        [Required]
        public DateTime EndDataOfCarLic { get; set; }

        // relations 
        public String User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }

        
    }
}
