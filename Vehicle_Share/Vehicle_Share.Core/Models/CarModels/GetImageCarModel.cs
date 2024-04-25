using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.CarModels
{
    public class GetImageCarModel
    {
        public string Id { get; set; }
        public string? Image { get; set; }
        public string? LicenseImageFront { get; set; }
        public string? LicenseImageBack { get; set; }
    }
}
