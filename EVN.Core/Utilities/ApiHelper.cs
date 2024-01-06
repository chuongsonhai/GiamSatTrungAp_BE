using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public static class ApiHelper
    {
        static ILog log = LogManager.GetLogger(typeof(ApiHelper));
        
        public static SignResponseData SignApi(string data, out string messages)
        {
            messages = "";
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var caConfigs = cfgservice.GetDictionary("CA_");

                string caApiUrl = caConfigs["CA_URL"];
                string caApiUser = caConfigs["CA_USER"];
                string caApiPass = caConfigs["CA_PASS"];

                var client = new RestClient(caApiUrl);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);

                var plainTextBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", caApiUser, caApiPass));
                string val = Convert.ToBase64String(plainTextBytes);
                request.AddHeader("Authorization", "Basic " + val);

                IRestResponse response = client.Execute(request);
                log.Error($"Status: {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    messages = "Không có quyền truy cập";
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.StatusDescription}");
                    messages = $"{response.StatusCode}: {response.StatusDescription}";
                    return null;
                }
                SignResponseData result = JsonConvert.DeserializeObject<SignResponseData>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public static CreateHashHopDongData CreateHashHopDongUSB(string TuKhoa, string cert, string Base64File, out string messages)
        {
            messages = "";
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var caConfigs = cfgservice.GetDictionary("CA_");

                string caCreateHashApiUrl = caConfigs["CA_CreateHashHopDong_USB"];
                string caApiUser = caConfigs["CA_USER"];
                string caApiPass = caConfigs["CA_PASS"];

                var client = new RestClient(caCreateHashApiUrl);
                var request = new RestRequest();
                CreateHashHopDongRequest createHashHopDongRequest = new CreateHashHopDongRequest();
                createHashHopDongRequest.cert = cert;
                createHashHopDongRequest.TuKhoa = TuKhoa;
                createHashHopDongRequest.Base64File = Base64File;
                var jsondata = JsonConvert.SerializeObject(createHashHopDongRequest);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsondata, ParameterType.RequestBody);

                var plainTextBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", caApiUser, caApiPass));
                string val = Convert.ToBase64String(plainTextBytes);
                request.AddHeader("Authorization", "Basic " + val);

                IRestResponse response = client.Execute(request);
                log.Error($"Status: {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    messages = "Không có quyền truy cập";
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.StatusDescription}");
                    messages = $"{response.StatusCode}: {response.StatusDescription}";
                    return null;
                }
                CreateHashHopDongData result = JsonConvert.DeserializeObject<CreateHashHopDongData>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public static HopDongInsertHashedFileData HopDongInsertHashedFile_USB(string id_file, string hashed, out string messages)
        {
            messages = "";
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var caConfigs = cfgservice.GetDictionary("CA_");

                string caHopDongInsertHashedFileApiUrl = caConfigs["CA_HopDongInsertHashedFile_USB"];
                string caApiUser = caConfigs["CA_USER"];
                string caApiPass = caConfigs["CA_PASS"];

                var client = new RestClient(caHopDongInsertHashedFileApiUrl);
                HopDongInsertHashedFileRequest hopDongInsertHashedFileRequest = new HopDongInsertHashedFileRequest();
                hopDongInsertHashedFileRequest.hashed= hashed;
                hopDongInsertHashedFileRequest.id_file= id_file;
                var jsondata = JsonConvert.SerializeObject(hopDongInsertHashedFileRequest);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsondata, ParameterType.RequestBody);

                var plainTextBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", caApiUser, caApiPass));
                string val = Convert.ToBase64String(plainTextBytes);
                request.AddHeader("Authorization", "Basic " + val);

                IRestResponse response = client.Execute(request);
                log.Error($"Status: {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    messages = "Không có quyền truy cập";
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.StatusDescription}");
                    messages = $"{response.StatusCode}: {response.StatusDescription}";
                    return null;
                }
                HopDongInsertHashedFileData result = JsonConvert.DeserializeObject<HopDongInsertHashedFileData>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public static string SyncApi(string action)
        {
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var syncConfigs = cfgservice.GetDictionary("SYNC_");
                string syncUrl = syncConfigs["SYNC_URL"];
                var client = new RestClient(syncUrl);
                var request = new RestRequest(action);
                request.Method = Method.GET;
                request.AddHeader("Content-Type", "application/json");

                IRestResponse response = client.Execute(request);
                log.Error($"Status: {response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.Content}");
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

        public static string PostApi(string action, string maYCau)
        {
            try
            {
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var syncConfigs = cfgservice.GetDictionary("SYNC_");
                string syncUrl = syncConfigs["SYNC_URL"];

                JObject data = new JObject();
                data["maYCau"] = maYCau;
                
                var client = new RestClient(syncUrl);
                var request = new RestRequest(action);

                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.Content}");
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

        public static string PostData(string url, string action, string data)
        {
            try
            {
          
                var client = new RestClient(url);
                var request = new RestRequest(action);
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", data, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    log.Error("Hết phiên, đăng nhập và chạy lại");
                    return null;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    log.Error($"{response.StatusCode}: {response.Content}");
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
    }

    public class SignResponseData
    {
        public bool suc { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }
    public class CreateHashHopDongData
    {
        public bool suc { get; set; }
        public string id_file { get; set; }
        public string hashed { get; set; }
        public string msg { get; set; }
    }
    public class CreateHashHopDongRequest
    {
        public string TuKhoa { get; set; }
        public string cert { get; set; }
        public string Base64File { get; set; }
    }
    public class HopDongInsertHashedFileData
    {
        public bool suc { get; set; }
        public string filebase64 { get; set; }
        public string msg { get; set; }
    }
    public class HopDongInsertHashedFileRequest
    {
        public string id_file { get; set; }
        public string hashed { get; set; }
    }
}
