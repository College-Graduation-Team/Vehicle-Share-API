using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.CarModels
{
    public class GetCarModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public int Model { get; set; }
        public string Brand { get; set; }
        public string CarPlate { get; set; }
        public int SetsOfCar { get; set; }

        public string CarImg { get; set; }
        public string LicImgCarFront { get; set; }
        public string LicImgCarBack { get; set; }
        public DateTime EndDataOfCarLic { get; set; }
    }
}
