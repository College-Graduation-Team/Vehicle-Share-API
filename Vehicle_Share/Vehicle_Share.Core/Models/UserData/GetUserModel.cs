﻿namespace Vehicle_Share.Core.Models.UserData
{
    public class GetUserModel
    {
    
        public string Id { get; set; }
        public string Name { get; set; }
        public long NationailId { get; set; }
        public int Age { get; set; }
        public bool? Gender { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string NationalCardImageFront { get; set; }
        public string NationalCardImageBack { get; set; }
        public string ProfileImage { get; set; }
        public bool Type { get; set; }

    }
}
