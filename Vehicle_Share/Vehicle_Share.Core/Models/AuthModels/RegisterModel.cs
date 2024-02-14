using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
