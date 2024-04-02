namespace Vehicle_Share.Core.Models.UserData
{
    public class GetUserModel
    {
    
        public string Id { get; set; }
        public string FullName { get; set; }
        public long NationailID { get; set; }
        public int Age { get; set; }
        public bool? Gender { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string NationalcardImgFront { get; set; }
        public string NationalcardImgBack { get; set; }
        public string ProfileImg { get; set; }
        public bool typeOfUser { get; set; }

    }
}
