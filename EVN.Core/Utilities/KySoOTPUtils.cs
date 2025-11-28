using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVN.Core.Utilities
{
    public static class KySoOTPUtils
    {
        static ILog log = LogManager.GetLogger(typeof(KySoOTPUtils));
        public static ApiResult LayMaOTP(CreateAndSendOtpCmisCommand request, CancellationToken cancellationToken)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            if (string.IsNullOrWhiteSpace(request.noiDung))
            {
                request.noiDung =
                    "Ma OTP cua quy khach hang la #OTP. Ma OTP nay se het hieu luc sau 03 phut.Hotline:19001288";
            }

            string url = "http://gwlocal.evnhanoi.vn/otp/api/Otp/send-otp-sms-email";

            var client = new RestClient(url);
            client.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator("otp", "otp");

            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Accept", "*/*");
            restRequest.AddHeader("apikey", "VKf8YECodV");

            // Gửi đúng 3 field giống Postman
            var body = new
            {
                soDienThoai = request.soDienThoai,
                maDonVi = request.maDonVi,
                noiDung = request.noiDung
            };
            restRequest.AddJsonBody(body);

            IRestResponse response = client.Execute(restRequest);
            string content = response.Content;

            var evnResult = JsonUtilSerializer.FromJsonString<BaseOtpResponseWrapper>(content);
            var returnObject = new ApiResult
            {
                IsError = evnResult.data.errorCode != "0",
                Data = evnResult.data.token,
                Message = evnResult.data.message
            };
            return returnObject;
        }


        public static ApiResult XacNhanOTP(VerifyOtpCmisCommand request, CancellationToken cancellationToken)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                return true;
                };
            string url = "http://gwlocal.evnhanoi.vn/otp/api/Otp/verify-otp-sms-email";

            var client = new RestClient(url);
            client.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator("otp", "otp");

            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Accept", "*/*");
            restRequest.AddHeader("apikey", "VKf8YECodV");

            // Gửi đúng 3 field giống Postman
            var body = new
            {
                SoDienThoai = request.SoDienThoai,
                MaOTP = request.MaOtp,
                MaToken  = request.Token,
                MaDonVi = request.MaDonVi,
                Noidung = "Xác nhận OTP",
            };
            restRequest.AddJsonBody(body);

            IRestResponse response = client.Execute(restRequest);
            log.ErrorFormat("Response: {0}", JsonConvert.SerializeObject(response));
            string content =  response.Content;
            var evnResult = JsonUtilSerializer.FromJsonString<BaseOtpResponseWrapper>(content);
            var returnObject = new ApiResult
            {
                IsError = evnResult.data.errorCode != "0",
                Data = evnResult.data.token,
                Message = evnResult.data.message
            };

            if (!returnObject.IsError)
            {
                // _mediator.Send(new InsertOtpAndTokenCommand()
                //{
                //    Otp = request.totp,
                //    Token = request.token,
                //    Email = request.userId,
                //    DienThoai = request.userId
                //});
            }
            return returnObject;
        }

        public static ApiResult KySoOTP(KySoOTPTrungApCommand request, CancellationToken cancellationToken)
        {

            var result = new ApiResult()
            {
                IsError = true,
                Data = null,
            };
            string url = "http://10.9.125.104:6064";
            var client = new RestClient($"{url}/KySo_OTP");
            var restRequest = new RestRequest();
            restRequest.Method = Method.POST;
            restRequest.AddHeader("Content-Type", "application/json");
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes("EVNHANOI:Evnhanoi@123"));
            restRequest.AddHeader("Authorization", "Basic " + authString);
            string jsonRequest = JsonConvert.SerializeObject(request);

            restRequest.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);
            IRestResponse response = client.Execute(restRequest);
            string content =  response.Content;
            var resultKySo = JsonUtilSerializer.FromJsonString<KySoOTPTrungApResultDto>(content);

            result = new ApiResult
            {
                IsError = resultKySo.IsError,
                Message = resultKySo.Message,
                Data = resultKySo.Data,
            };
            //TODO: lưu log4net
            return result;
        }
    }
}
