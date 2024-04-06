using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.LicModels
{
    public class LicModel
    {
        [Required(ErrorMessage = "Image is required.")]
        public IFormFile ImageFront { get; set; }


        [Required(ErrorMessage = "Image is required.")]
        public IFormFile ImageBack { get; set; }

        [Required]
        public DateTime Expiration { get; set; }
    }
}
