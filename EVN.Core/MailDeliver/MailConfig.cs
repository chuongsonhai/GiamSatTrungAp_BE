using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace EVN.Core
{
    public class MailConfig
    {
        public MailConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\MailConfig.json";
            using (StreamReader file = new StreamReader(path))
            {
                string content = file.ReadToEnd();
                var data = JsonConvert.DeserializeObject<JObject>(content);
                host = data["MAIL_HOST"].ToString();
                port = data["MAIL_PORT"].ToString();
                mailSender = data["MAIL_ACC"].ToString();
                mailPassword = data["MAIL_PASS"].ToString();
                enableSsl = data["MAIL_SSL"].ToString();
            }
        }
        public string host { get; set; }
        public string port { get; set; }
        public string mailSender { get; set; }
        public string mailPassword { get; set; }
        public string enableSsl { get; set; }
    }
}
