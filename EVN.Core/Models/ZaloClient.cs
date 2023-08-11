using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EVN.Core.Models
{
    public class ZaloClient 
    {


        string access_token = "";
        public string strHTML, strXML, strHTMLTB, Bodyhtml, email, bHoaDon, ma_kh, noidung_file, substring;
        int so_luong;
        string ketqua_guitin, cu_phap, chude, ChuY, Uid, id_zalo;
        string time = DateTime.Now.ToString("h:mm:ss tt");
        private byte[] buffer, bchuky, bchuky1;

        JObject result_guitin; //= new JObject();

        public string Url { get; set; }

        string Api_Guitinnhan = "https://openapi.zalo.me/v2.0/oa/message";
        //string Api_Guitinnhan = "http://192.168.199.5:85/v2.0/oa/message";

        string Api_Getthongtin = "https://openapi.zalo.me/v2.0/oa/getprofile";
        //string Api_Getthongtin = "http://192.168.199.5:85/v2.0/oa/getprofile";

        public string Access_token
        {
            get;
            set;
        }

        public string Send_Zalo_OA(string sdt, string noidung)
        {
            // sdt = "84989836333";
            // get_uid(sdt);
            getProfileOfFollower(sdt);
            sendTextMessageToUserId(id_zalo, noidung);
            ketqua_guitin = result_guitin.ToString();

            return ketqua_guitin;
        }

        //Tu
        public JObject excuteRequest(string method, string endPoint, Dictionary<string, dynamic> param)
        {
            DataProvide_Oracle cnn = new DataProvide_Oracle();
            DataTable dt = new DataTable();
            dt = cnn.Get_token_zalo();
            access_token = dt.Rows[0]["TOKEN"].ToString().Trim();

            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;


            //access_token = "0QsZ37ot10K0hQODUha02G2de05zfK5mSyUt4d-VQHr2-RzEMf9m7rlceLX1i78IR_sCIs6bUqL9nv4eQz908d-zcoXyfsX_SkJ89rAI3LiozEm_5hqBRGFBmZO0rn5xEgtB4Hpl0Ketyz004vToLdFIbHfskcn_Ng6DBcFKLMTAeOOICFexIXo5uY4pmIPT2wJr0oF9LbS7gPDPECDM9nE1aIeycJP018tGC0pf37aeZjudA-uUI12Bspzip1rGPRMW9rgjT7bVvuWsP9iXQX_Yz6Gcf15ePLDDB90zVwy70m";

            if (param == null)
            {
                param = new Dictionary<string, dynamic>();
            }
            param.Add("access_token", access_token);
            string response;

            if ("GET".Equals(method.ToUpper()))
            {
                response = sendHttpGetRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
            }
            else
            {
                if (param.ContainsKey("file"))
                {
                    response = sendHttpUploadRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
                }
                else if (param.ContainsKey("body"))
                {
                    response = sendHttpPostRequestWithBody(endPoint, param, param["body"], APIConfig.DEFAULT_HEADER);
                }
                else
                {
                    response = sendHttpPostRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
                }
            }

            JObject result = null;
            try
            {
                result = JObject.Parse(response);
            }
            catch (Exception e)
            {
                //  throw new APIException("Response is not json: " + response);
            }
            return result;
        }


        public string get_idzalo(string user_id)
        {
            JObject result = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

            JObject dataJson = JObject.FromObject(new
            {
                user_id
            });

            param.Add("data", dataJson.ToString());

            result = excuteRequest("GET", Api_Getthongtin, param);

            if ((result.ToString().Contains("-213")) || (result.ToString().Contains("-201")))
            {
               // id_zalo = "-1";
            }
            else
            {
                UID_OA Uiddata = JsonConvert.DeserializeObject<UID_OA>(result.ToString());
                id_zalo = Uiddata.data.user_id.ToString();
            }

            return id_zalo;
        }

        protected string sendHttpPostRequest(string endpoint, Dictionary<string, dynamic> param, Dictionary<string, string> header)
        {
            Dictionary<string, string> paramsUrl = new Dictionary<string, string>();
            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }
            if (param != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in param)
                {
                    if (entry.Value is string)
                    {
                        paramsUrl[entry.Key] = entry.Value;
                    }
                }
            }
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(paramsUrl);
            if (isDebug)
            {
                UriBuilder builder = new UriBuilder(endpoint);
                var query = HttpUtility.ParseQueryString(builder.Query);
                foreach (KeyValuePair<string, string> entry in paramsUrl)
                {
                    query[entry.Key] = entry.Value;
                }
                builder.Query = query.ToString();
                Console.WriteLine("POST: " + builder.ToString());
            }
            HttpResponseMessage response = httpClient.PostAsync(endpoint, formUrlEncodedContent).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        protected string sendHttpUploadRequest(string endpoint, Dictionary<string, dynamic> param, Dictionary<string, string> header)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            UriBuilder builder = new UriBuilder(endpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (param != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in param)
                {
                    if (entry.Value is string)
                    {
                        query[entry.Key] = entry.Value;
                    }
                }
            }
            builder.Query = query.ToString();

        //    ZaloFile file = param["file"];
          //  form.Add(file.GetData(), "file", file.GetName());

            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }

            HttpResponseMessage response = httpClient.PostAsync(builder.ToString(), form).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        protected string sendHttpPostRequestWithBody(string endpoint, Dictionary<string, dynamic> param, string body, Dictionary<string, string> header)
        {
            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }

            UriBuilder builder = new UriBuilder(endpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (param != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in param)
                {
                    if (entry.Value is string)
                    {
                        query[entry.Key] = entry.Value;
                    }
                }
            }
            builder.Query = query.ToString();

            if (body == null)
            {
                body = "";
            }
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            if (isDebug)
            {
                Console.WriteLine("POST: " + builder.ToString());
                Console.WriteLine("body: " + body);
                Console.WriteLine("body content: " + content);
            }
            HttpResponseMessage response = httpClient.PostAsync(builder.ToString(), content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        public bool isDebug = false;
        protected string sendHttpGetRequest(string endpoint, Dictionary<string, dynamic> param, Dictionary<string, string> header)
        {
            UriBuilder builder = new UriBuilder(endpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (param != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in param)
                {
                    if (entry.Value is string)
                    {
                        query[entry.Key] = entry.Value;
                    }
                }
            }
            builder.Query = query.ToString();

            HttpClient httpClient = new HttpClient();
            if (header != null)
            {
                foreach (KeyValuePair<string, string> entry in header)
                {
                    httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }
            if (isDebug)
            {
                Console.WriteLine("GET: " + builder.ToString());
            }
            return httpClient.GetStringAsync(builder.ToString()).Result;
        }






        //public JObject excuteRequest_Bak(string method, string endPoint, Dictionary<string, dynamic> param)
        //{
        //    //access_token = "-QH8EtGe8Ltuf4qwSYObKVYkIpPrEb08ikLfCdboJbQjlN5tILD6VuIjIXvyNruDYu0p9KnNAW6XY5aNBGDVTlxRUtCt95HfvT0uOZSw9LljpKbXQ1LUHRJtULDp8a1BgEfyKbCuKMwanpXnQGqh3hF_FHfx15iSy-5h22neGWAWitDdNqO0RxEG7tDVOtHakwjPP6ndHWAIatzaTdv2NegUBbPlQaPRuQftI0Xu66dSz2qMLYCMDDxw71On6JSRtiXO92GOGKxzqsTJCX1uCSd92quU1p1LmCuZQhswNb9tC29R";
        //    access_token = "0QsZ37ot10K0hQODUha02G2de05zfK5mSyUt4d-VQHr2-RzEMf9m7rlceLX1i78IR_sCIs6bUqL9nv4eQz908d-zcoXyfsX_SkJ89rAI3LiozEm_5hqBRGFBmZO0rn5xEgtB4Hpl0Ketyz004vToLdFIbHfskcn_Ng6DBcFKLMTAeOOICFexIXo5uY4pmIPT2wJr0oF9LbS7gPDPECDM9nE1aIeycJP018tGC0pf37aeZjudA-uUI12Bspzip1rGPRMW9rgjT7bVvuWsP9iXQX_Yz6Gcf15ePLDDB90zVwy70m";

        //    if (param == null)
        //    {
        //        param = new Dictionary<string, dynamic>();
        //    }
        //    param.Add("access_token", access_token);
        //    string response;

        //    if ("GET".Equals(method.ToUpper()))
        //    {
        //        response = sendHttpGetRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
        //    }
        //    else
        //    {
        //        if (param.ContainsKey("file"))
        //        {
        //            response = sendHttpUploadRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
        //        }
        //        else if (param.ContainsKey("body"))
        //        {
        //            response = sendHttpPostRequestWithBody(endPoint, param, param["body"], APIConfig.DEFAULT_HEADER);
        //        }
        //        else
        //        {
        //            response = sendHttpPostRequest(endPoint, param, APIConfig.DEFAULT_HEADER);
        //        }
        //    }

        //    JObject result = null;
        //    try
        //    {
        //        result = JObject.Parse(response);
        //    }
        //    catch (Exception e)
        //    {
        //        //  throw new APIException("Response is not json: " + response);
        //    }
        //    return result;
        //}

        //private string sendHttpGetRequest(string endPoint, Dictionary<string, dynamic> param, Dictionary<string, string> dEFAULT_HEADER)
        //{
        //    throw new NotImplementedException();
        //}

        public class default_action1
        {
            public string type { get; set; }
            public string url { get; set; }
        }

        public class elements1
        {
            public string title { get; set; }
            public string subtitle { get; set; }
            public string image_url { get; set; }
            public default_action1 default_action { get; set; }
        }

        public JObject sendButtonMessageToUserId(string user_id, string content, string Url, string TieuDe)
        {

            result_guitin = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

            List<elements1> ListElements;
            ListElements = new List<elements1>();

            ListElements.Add(new elements1 { title = TieuDe, subtitle = content, image_url = "https://dangkyzalo.evnhanoi.com.vn/images/thongbaotiendienthang.png", default_action = new default_action1 { type = "oa.open.url", url = Url } });
            ListElements.Add(new elements1 { title = "Thanh toán ngay bằng ZaloPay", subtitle = content, image_url = "https://lh3.googleusercontent.com/F8cUV5oOLjCTMSvSRymK1154MwKalnvkepN4xGrfWBC_tcXvNTq_sEStiwCYV61lRdI", default_action = new default_action1 { type = "oa.open.url", url = Url } });
            // ListElements.Add(new elements1 { title = "Xem ảnh ghi chỉ số công tơ", subtitle = content, image_url = "https://lh5.googleusercontent.com/proxy/hlFpndLmIPuabPmpyzzvzoDRN7k8VPJB2bH2nW71PxMLQjst485Wh1SQrn-g689dn2ZB7AzVLQWYwiAJu3mckVgw5mAb-XFz8VszFdX3VH8LTLyY0-bs0Q", default_action = new default_action1 { type = "oa.open.url", url = "http://cskh.evnhanoi.com.vn/TraCuuChiSo/Index" } });

            JObject body = JObject.FromObject(new
            {
                recipient = new
                {
                    user_id
                },
                message = new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "list",
                            elements = ListElements,

                        }
                    }
                }

            });
            param.Add("body", body.ToString());

            result_guitin = excuteRequest("POST", Api_Guitinnhan, param);

            return result_guitin;
        }

        public JObject sendButtonMessageToUserId_Bak(string user_id, string content, string Url, string TieuDe)
        {

            result_guitin = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

            List<elements1> ListElements;
            ListElements = new List<elements1>();

            ListElements.Add(new elements1 { title = TieuDe, subtitle = content, image_url = "https://lh5.googleusercontent.com/proxy/hlFpndLmIPuabPmpyzzvzoDRN7k8VPJB2bH2nW71PxMLQjst485Wh1SQrn-g689dn2ZB7AzVLQWYwiAJu3mckVgw5mAb-XFz8VszFdX3VH8LTLyY0-bs0Q", default_action = new default_action1 { type = "oa.open.url", url = Url } });
            ListElements.Add(new elements1 { title = "Thanh toán ngay bằng ZaloPay", subtitle = content, image_url = "https://lh3.googleusercontent.com/F8cUV5oOLjCTMSvSRymK1154MwKalnvkepN4xGrfWBC_tcXvNTq_sEStiwCYV61lRdI", default_action = new default_action1 { type = "oa.open.url", url = Url } });
            // ListElements.Add(new elements1 { title = "Xem ảnh ghi chỉ số công tơ", subtitle = content, image_url = "https://lh5.googleusercontent.com/proxy/hlFpndLmIPuabPmpyzzvzoDRN7k8VPJB2bH2nW71PxMLQjst485Wh1SQrn-g689dn2ZB7AzVLQWYwiAJu3mckVgw5mAb-XFz8VszFdX3VH8LTLyY0-bs0Q", default_action = new default_action1 { type = "oa.open.url", url = "http://cskh.evnhanoi.com.vn/TraCuuChiSo/Index" } });

            JObject body = JObject.FromObject(new
            {
                recipient = new
                {
                    user_id
                },
                message = new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "list",
                            elements = ListElements,

                        }
                    }
                }

            });
            param.Add("body", body.ToString());

            //   hd = new ZaloClient();

            result_guitin = excuteRequest("POST", Api_Guitinnhan, param);


            return result_guitin;
        }






        public JObject sendTextMessageToUserId(string user_id, string content)
        {
            result_guitin = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

            JObject body = JObject.FromObject(new
            {
                recipient = new
                {
                    user_id
                },
                message = new
                {
                    text = content
                }
            });
            param.Add("body", body.ToString());

            result_guitin = excuteRequest("POST", Api_Guitinnhan, param);


            return result_guitin;
        }

        public JObject sendInviteToUserId(string phone, string ma_khang)
        {
            JObject result = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();
            //  JObject data = new JObject();

            JObject data = JObject.FromObject(new
            {
                tracking_source = "ma_khang"
            });

            List<JObject> elementJson = new List<JObject>();
            elementJson.Add(JObject.FromObject(new
            {
                template_id = "83e4c20afe4f17114e5e",
                template_data = data,
                // payload = "callback_data"

                // attachment_id = image_attachment_id
            }));

            JObject body = JObject.FromObject(new
            {
                recipient = new
                {
                    phone
                },
                message = new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "invite",
                            elements = elementJson

                        }
                    }
                }

            });
            param.Add("body", body.ToString());

            result = excuteRequest("POST", Api_Guitinnhan, param);

            return result;
        }

        public class UID_OA
        {
            public Data data { get; set; }
            public int error { get; set; }
            public string message { get; set; }
        }

        public class Data
        {
            public string user_gender { get; set; }
            public string user_id { get; set; }
        }

        public JObject getProfileOfFollower(string user_id)
        {
            JObject result = new JObject();
            Dictionary<string, dynamic> param = new Dictionary<string, dynamic>();

            JObject dataJson = JObject.FromObject(new
            {
                user_id
            });

            param.Add("data", dataJson.ToString());

            result = excuteRequest("GET", Api_Getthongtin, param);

            UID_OA Uiddata = JsonConvert.DeserializeObject<UID_OA>(result.ToString());

            id_zalo = Uiddata.data.user_id.ToString();

            return result;
        }

  

        public string get_uid(String id)
        {
            string oaid = "1373748071056591217";
            string timestamp = "1502363781000";
            string uid = id;
            //  string uid = "84989836333";
            string getmac = oaid + uid + timestamp + "ml8SCWOaPyki1N0M7bZM";
            string mac = sha256_hash(getmac);

            Url = "https://openapi.zaloapp.com/oa/v1/getprofile";

            var name_values = new NameValueCollection();
            name_values.Add("oaid", oaid);
            name_values.Add("uid", uid);
            name_values.Add("timestamp", timestamp.ToString());
            name_values.Add("mac", mac);


            var url_params = ToQueryString(name_values);
            WebRequest request = WebRequest.Create(Url + url_params);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            Uid = reader.ReadToEnd();
            // Display the content.
            // Console.WriteLine(result);
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

            return Uid;

        }



        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }
    }
}

