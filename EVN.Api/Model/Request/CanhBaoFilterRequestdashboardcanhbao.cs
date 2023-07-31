using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{

    public class CanhBaoFilterRequestdashboardcanhbao : BaseRequest
    {
        [JsonProperty("filterdashboardcanhbao")]
        public dashboardcanhbao Filterdashboardcanhbao { get; set; }
    }

    public class dashboardcanhbao
    {

        public string fromdate { get; set; } = string.Empty;
        public string todate { get; set; } = string.Empty;
    }

}
