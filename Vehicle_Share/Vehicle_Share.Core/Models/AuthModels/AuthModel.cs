using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuth { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public bool PhoneConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }

        public string TokenExpiration { get; set; }

        // [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}
