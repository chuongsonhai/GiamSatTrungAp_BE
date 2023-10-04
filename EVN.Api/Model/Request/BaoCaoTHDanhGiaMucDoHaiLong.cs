using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{

    public class BaoCaoTHDanhGiaMucDoHaiLong : BaseRequest
    {
        [JsonProperty("FilterDGiaDoHaiLong")]
        public FilterDanhGiaMucDoHaiLong FilterDGiaDoHaiLong { get; set; }

        public class FilterDanhGiaMucDoHaiLong
        {
            public string maDViQLy { get; set; }
            public string fromdate { get; set; }
            public string todate { get; set; }
            public int HangMucKhaoSat { get; set; }
        }
    }
}