using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;

namespace EVN.Core
{
    public class DOfficeUtils
    {
        static ILog log = LogManager.GetLogger(typeof(DOfficeUtils));
        public static VanBanResponse GetDocument(VanBanRequest data)
        {
            try
            {
                var jsondata = JsonConvert.SerializeObject(data);
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var doConfigs = cfgservice.GetDictionary("DO_");

                string doApiUrl = doConfigs["DO_API"];
                string doApiUser = doConfigs["DO_USER"];
                string doApiPass = doConfigs["DO_PASS"];

                var client = new RestClient(doApiUrl);
                var request = new RestRequest("/api/GET_VANBAN");
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsondata, ParameterType.RequestBody);

                var byteArray = Encoding.ASCII.GetBytes($"{doApiUser}:{doApiPass}");
                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(byteArray));

                IRestResponse response = client.Execute(request);
                log.Error($"Status: {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.Content}");
                    log.Error($"{response.StatusCode}: {response.StatusDescription}");
                    return null;
                }
                var result = JsonConvert.DeserializeObject<VanBanResponse>(response.Content);
                if (result == null || string.IsNullOrWhiteSpace(result.DATA)) return null;
                log.Error($"{result.MESSAGE}");
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
    }

    public class VanBanRequest
    {
        public VanBanRequest() { }
        public VanBanRequest(string sovban, DateTime ngayvban) : base()
        {
            this.SoVanBan = sovban;
            this.NgayVanBan = ngayvban.ToString("MM/dd/yyyy");
        }
        public string SoVanBan { get; set; }
        public string NgayVanBan { get; set; }
    }

    public class VanBanResponse
    {
        public string STATUS { get; set; } = "OKE";
        public string MESSAGE { get; set; }
        public string DATA { get; set; }
    }
}
