using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

            string stringInput = "";
            //request.userId = "0917709015"; //0912312530
            //request.content =
            //    "Ma OTP cua Giao dich ky so ho so mua ban dien cua KH la %23OTP. Ma OTP nay se het hieu luc sau 03 phut.Hotline:19001288";
            if (string.IsNullOrWhiteSpace(request.content))
            {
                request.content =
                    "Ma OTP cua quy khach hang la %23OTP. Ma OTP nay se het hieu luc sau 03 phut.Hotline:19001288";
            }
            Type myType = request.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            int i = 1;
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                stringInput += (i == 1) ? string.Empty : "&";
                stringInput += prop.Name + "=" + propValue;
                i++;
                // Do something with propValue
            }


            string url = "http://10.9.125.71:6973/otp";
            var client = new RestClient($"{url}/PushQueueService/vn/com/evn/otp/generate.wadl?" + stringInput);
            var restRequest = new RestRequest();
            restRequest.Method = Method.POST;
            restRequest.AddHeader("Content-Type", "application/json");


            IRestResponse response = client.Execute(restRequest);            
            string content =  response.Content;
            var evnResult = JsonUtilSerializer.FromJsonString<BaseOtpWebServiceResultDto>(content);
            var returnObject = new ApiResult
            {
                IsError = evnResult.ErrorCode != "0",
                Data = evnResult.Token,
                Message = evnResult.Message
            };
            return returnObject;
        }

        public static ApiResult XacNhanOTP(VerifyOtpCmisCommand request, CancellationToken cancellationToken)
        {

            string stringInput = "";
            Type myType = request.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            int i = 1;
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                stringInput += (i == 1) ? string.Empty : "&";
                stringInput += prop.Name + "=" + propValue;
                i++;
                // Do something with propValue
            }
            string url = "http://10.9.125.71:6973/otp";
            var client = new RestClient($"{url}/PushQueueService/vn/com/evn/otp/verify.wadl?" + stringInput);
            log.ErrorFormat("XacNhanOTP: {0}", JsonConvert.SerializeObject(client));
            var restRequest = new RestRequest();
            restRequest.Method = Method.POST;
            restRequest.AddHeader("Content-Type", "application/json");

            IRestResponse response = client.Execute(restRequest);
            log.ErrorFormat("Response: {0}", JsonConvert.SerializeObject(response));
            string content =  response.Content;
            var evnResult = JsonUtilSerializer.FromJsonString<BaseOtpWebServiceResultDto>(content);
            var returnObject = new ApiResult
            {
                IsError = evnResult.ErrorCode != "0",
                Data = evnResult.Token,
                Message = evnResult.Message
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
            string url = "http://10.9.125.104:6064/";
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
