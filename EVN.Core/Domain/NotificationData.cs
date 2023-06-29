using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class NotificationData
    {
        public NotificationData(string appid, string content)
        {
            app_id = appid;
            contents = new { en = content };
        }
        public string app_id { get; set; }
        public object contents { get; set; }
        public string[] include_player_ids { get; set; }
    }
}
