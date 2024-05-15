using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class SearchModel
    {
        public double? FromLatitude { get; set; }
        public double? FromLongitude { get; set; }

        public double? ToLatitude { get; set; }
        public double? ToLongitude { get; set; }

        public DateTime? StartDate { get; set; } 
    }
}
