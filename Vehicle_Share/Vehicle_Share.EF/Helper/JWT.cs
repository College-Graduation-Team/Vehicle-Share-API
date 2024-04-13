namespace Vehicle_Share.EF.Helper
{
    public class JWT
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
        public double DurationInMinutes { get; set; }

    }
}
