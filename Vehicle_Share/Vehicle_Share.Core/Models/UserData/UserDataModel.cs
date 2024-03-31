using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.UserData
{
    public class UserDataModel
    {   
        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }


        [Required(ErrorMessage = "Nationail ID is required.")]
        [RegularExpression(@"^\d{14}$",ErrorMessage = "Nationail ID must be 14 digit .")]
        public long NationailID { get; set; }


        public DateTime? BirthData { get; set; }


        public  bool? Gender { get; set; }
        public  bool typeOfUser { get; set; }


        [Required(ErrorMessage = "Nationality is required.")]
        [MaxLength(50)]
        public  string Nationality { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile NationalcardImgFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile NationalcardImgBack { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile ProfileImg { get; set; }



    }
}
