using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace EVN.Core.CMIS
{
    public class ApiService
    {
        ILog log = LogManager.GetLogger(typeof(ApiService));
        public string CMISUrl { get; set; }
        public ApiService(string cmisUrl)
        {
            this.CMISUrl = cmisUrl;
        }

        public string PostData(string action, string data)
        {
            try
            {
                log.Error($"URL: {CMISUrl}");
                log.Error($"Action: {action}");
                var client = new RestClient(this.CMISUrl);
                var request = new RestRequest(action);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                log.Error("StatusCode:" + response.StatusCode);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error(response.ErrorMessage);
                    return null;
                }
                return response.Content;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ApiResponse UploadFile(string action, string data)
        {
            var client = new RestClient(this.CMISUrl);
            var request = new RestRequest(action);
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", data, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            log.Error("Content:" + response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return null;
            }
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            return result;
        }
    }

    public class ApiResponse
    {
        public string MESSAGE { get; set; }
        public string TYPE { get; set; }
    }
}
