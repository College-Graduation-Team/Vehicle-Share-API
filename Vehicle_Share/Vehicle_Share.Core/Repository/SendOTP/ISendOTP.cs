using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Vehicle_Share.Core.Repository.SendOTP
{
	public interface ISendOTP
	{
		MessageResource Send(string mobileNumber , string otp);

	}
}
