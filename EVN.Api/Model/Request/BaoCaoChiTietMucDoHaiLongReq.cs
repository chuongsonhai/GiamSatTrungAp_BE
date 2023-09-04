using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class BaoCaoChiTietMucDoHaiLongReq : BaseRequest
    {
        [JsonProperty("Filterbcmdhl")]
        public FilterBCMDHL Filterbcmdhl { get; set; }

        public class FilterBCMDHL
        {
            public string maDViQly { get; set; }
            public string fromdate { get; set; }
            public string todate { get; set; }
            public int HangMucKhaoSat { get; set; }
        }
    }
}