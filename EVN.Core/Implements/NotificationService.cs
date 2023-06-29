using EVN.Core.Domain;
using EVN.Core.IServices;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace EVN.Core.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(NotificationService));

        private string notificationURL;
        public NotificationService(string notificationURL)
        {
            this.notificationURL = notificationURL;
        }

        public void PushMessage(string apikey, string appid, string content, params string[] userids)
        {
            try
            {
                NotificationData data = new NotificationData(appid, content);
                data.include_player_ids = userids;

                var request = WebRequest.Create(notificationURL) as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("authorization", "Basic " + apikey);

                var jsondata = JsonConvert.SerializeObject(data);
                byte[] byteArray = Encoding.UTF8.GetBytes(jsondata);

                JObject objResult = new JObject();

                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JObject Jobject = JObject.Parse(reader.ReadToEnd());

                        objResult.Add("status", "SUCCESS");
                        objResult.Add("id", Jobject.GetValue("id"));
                        objResult.Add("recipients", Jobject.GetValue("recipients"));
                    }
                }
            }
            catch (WebException ex)
            {
                log.Error(ex);
            }
        }
    }
}
