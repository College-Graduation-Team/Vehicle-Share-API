using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Response
{
    public class StatusResponseModel
    {
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
