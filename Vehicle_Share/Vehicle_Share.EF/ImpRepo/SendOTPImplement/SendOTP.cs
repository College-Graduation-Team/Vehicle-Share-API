using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Vehicle_Share.Core.Repository.SendOTP;
using Vehicle_Share.EF.Helper;

namespace Vehicle_Share.EF.ImpRepo.SendOTPImplement
{
    public class SendOTP : ISendOTP
	{
		private readonly TwilioSettings _twilio;

		public SendOTP(IOptions<TwilioSettings> twilio)
		{
			_twilio = twilio.Value;
		}

		public MessageResource Send(string mobileNumber, string otp)
		{
			TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
			var message = $"This is Verification Code: {otp}";
			var result = MessageResource.Create(
					body: message,
					from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
					to: mobileNumber
				);
			return result;


		}
	}
}
