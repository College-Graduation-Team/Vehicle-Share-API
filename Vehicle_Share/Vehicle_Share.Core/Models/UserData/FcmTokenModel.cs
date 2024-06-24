using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Share.Core.Models.UserData
{
    public class FcmTokenModel
    {
        public string token { get; set; }
    }
}
