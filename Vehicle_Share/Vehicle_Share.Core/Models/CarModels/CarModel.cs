using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Vehicle_Share.Core.Models.CarModels
{
    public class CarModel
    {
        [Required]
        public string TypeOfCar { get; set; }


        [Required]
        public int ModelOfCar { get; set; }


        [Required]
        public string BrandOfCar { get; set; }

        [Required]
        public string PlateOfCar { get; set; }

        [Required]
        public Int16 CarSetNum { get; set; }

        // image of car 
        [Required]
        public IFormFile LecImgFront { get; set; }


        [Required]
        public IFormFile LecImgBack { get; set; }


        [Required]
        public IFormFile CarImg { get; set; }

        [Required]
        public DateTime EndDataOfCarLic { get; set; }


    }
}
