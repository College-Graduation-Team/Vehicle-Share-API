using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.TripModels
{
    public class UserIdsModel
    {
        public string driver { get; set; }
        public List<string> passengers { get; set; }
    }
}
