using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Response
{
    public static class ResponseCode
    {
        public const short NoUserData = 1;
        public const short NoCar = 2;
        public const short NoLicense = 3;
        public const short NoTrip = 4;
        public const short NoAuth = 5;
        public const short WrongPhoneOrPassword = 6;
        public const short NotConfirmPhoneNumber = 7;
        public const short ExistsUser = 8;
        public const short InValidPassword = 9;
        public const short InvalidPhone = 10;
        public const short ExistsPhone = 11;
    }
}
