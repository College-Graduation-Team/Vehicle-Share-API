using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class ResetPassModel
    {
        public string Phone  { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirm password is not match .")]
        public string ConfirmPassword { get; set; }
    }
}
