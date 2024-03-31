
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vehicle_Share.EF.Models
{
    public class UserData
    {
        [Key]
        public string UserDataID { get; set; }


        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }


        [Required(ErrorMessage = "Nationail ID is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Nationail ID must be 14 digit .")]
        public long NationailID { get; set; }


        public DateTime BirthData { get; set; }


        public  bool Gender { get; set; }



        [Required(ErrorMessage = "Nationality is required.")]
        [MaxLength(50)]
        public  string Nationality { get; set; }



        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string NationalcardImgFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string NationalcardImgBack { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public string ProfileImg { get; set; }

        public bool typeOfUser { get; set; }    // driver of passenger
        // relation 
        public String User_Id { get; set; }
        [ForeignKey("User_Id")]
        public User User { get; set; }

        public List<Car>? cars { get; set; }
    }
}
