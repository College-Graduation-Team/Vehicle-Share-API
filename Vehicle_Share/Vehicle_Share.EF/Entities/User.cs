using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class User : IdentityUser
    {
        public List<RefreshToken>? RefreshTokens { get; set; }
	    public string? ResetCode { get; set; }
        public DateTime? ResetCodeGeneateAt { get; set; }
		public bool ResetCodeExpired => DateTime.UtcNow >= ResetCodeGeneateAt?.AddMinutes(3);
        public UserData? UserData { get; set; }
    }
}
