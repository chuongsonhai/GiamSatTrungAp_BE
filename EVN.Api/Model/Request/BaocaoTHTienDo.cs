using EVN.Core;
using EVN.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class BaocaoTHTienDo : BaseRequest
    {
        [JsonProperty("Filterbctd")]
        public dashboardbctd Filterbctd { get; set; }
    }
    public class dashboardbctd
    {
        public string maDViQly { get; set; }
        public int TenLoaiCanhBao { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }


    }
}
