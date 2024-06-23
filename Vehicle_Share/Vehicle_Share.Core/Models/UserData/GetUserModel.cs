using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.Core.Models.UserData
{
    public class GetUserModel
    {

        public string? Id { get; set; }
        public string? Name { get; set; }
        public long NationalId { get; set; }
        public string? Birthdate { get; set; }
        public bool? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? ProfileImage { get; set; }
        public string? FcmToken { get; set; }

        public Status Status { get; set; }
        public string? Message { get; set; }

    }
}
