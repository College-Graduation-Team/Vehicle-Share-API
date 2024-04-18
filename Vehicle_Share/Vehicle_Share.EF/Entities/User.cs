using Microsoft.AspNetCore.Identity;

namespace Vehicle_Share.EF.Models
{
    public class User : IdentityUser
    {
        public List<RefreshToken>? RefreshTokens { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ResetCode { get; set; }
        public DateTime? ResetCodeGeneateAt { get; set; }
        public bool ResetCodeExpired => DateTime.UtcNow >= ResetCodeGeneateAt?.AddMinutes(3);
        public UserData? UserData { get; set; }
    }
}
