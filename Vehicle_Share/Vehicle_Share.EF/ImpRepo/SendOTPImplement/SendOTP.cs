
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Vehicle_Share.EF.ImpRepo.SendOTPImplement
{
    public class SendOTP //: ISendOTP
    {
        // private readonly TwilioSettings _twilio;
        const string Url = "https://www.sys-all.com/api";
        const string ApiKey = "WA-Cr8ppmzA3DQ3JpqEy4tDdvUYD3LK0n9Xua_TlNV8X3TlLH5CIcH4cr7Hxjei5";
        // you can use this online site https://codebeautify.org/image-to-base64-converter
        const string Base64Image = "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";

        /*public SendOTP(IOptions<TwilioSettings> twilio)
        {
            _twilio = twilio.Value;
        }*/

        #region MyRegion
        // public MessageResource Send(string mobileNumber, string otp)
        //   {
        /*TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        var message = $"This is Verification Code: {otp}";
        Console.WriteLine($"======================= {message} mobileNumber: {mobileNumber} =======================");
        // var result = MessageResource.Create(
        // 		body: message,
        // 		from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
        // 		to: mobileNumber
        // 	);
        // return result;
        return null;*/
        //  }
        #endregion

        #region Send Otp Api for DotnetCore/DotNetDesktop
        public static async Task<BaseResponseDto<EmptyResponseDto>> Send(string phone,string otp)
        {

            SendOtpRequestDto requestDto = new SendOtpRequestDto
            {
                Message = $"This is Verification Code: {otp}", // your message
                Number = phone //number must start with Country Code (e.g "20", "966", ...)

            };
            
            string serializedRequest = Newtonsoft.Json.JsonConvert.SerializeObject(new BaseRequestDto<SendOtpRequestDto>(requestDto));


            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(serializedRequest);
            System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(GenerateRequestUrl("Sender/SendOtp"));
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = bytes.Length;
            using (Stream stream = httpRequest.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            WebResponse webResponse = httpRequest.GetResponse();
            Console.WriteLine(((HttpWebResponse)webResponse).StatusDescription);
            var webData = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(webData);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            webData.Close();
            webResponse.Close();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponseDto<EmptyResponseDto>>(responseFromServer);

        }
        static string GenerateRequestUrl(string endPoint)
        {
            return Url + "/" + endPoint + "?apikey=" + ApiKey;
        }
        #endregion
    }
   
}

