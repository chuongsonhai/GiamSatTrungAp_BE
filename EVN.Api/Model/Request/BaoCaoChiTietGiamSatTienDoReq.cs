using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class BaoCaoChiTietGiamSatTienDoReq
    {
        
            [JsonProperty("FilterBaoCaoGiamSatTienDo")]
            public FilterBCGSTD Filterbcgstd { get; set; }
        
        public class FilterBCGSTD
        {
            public string maDViQly { get; set; }
            public string fromdate { get; set; }
            public string todate { get; set; }
        }
    }
}