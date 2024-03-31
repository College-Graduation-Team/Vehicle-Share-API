using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripDriverModel
    {
        public string id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public float Recommendprice { get; set; }
        public int AvilableSets { get; set; } //driver
        public string CarID { get; set; } //driver
    }
}
