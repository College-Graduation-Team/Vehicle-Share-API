using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class PhoneModel
    {
        public string Phone { get; set; }

        public string Token { get; set; }
    }
}
