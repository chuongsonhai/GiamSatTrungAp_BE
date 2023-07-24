using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class PhanhoiTraodoiFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public PhanhoiTraodoiFilter Filter { get; set; }
    }

    public class PhanhoiTraodoiFilter
    {
        public int ID { get; set; }
        public int CANHBAO_ID { get; set; }
        public string NOIDUNG_PHANHOI { get; set; }
        public string NGUOI_GUI { get; set; }
        public string DONVI_QLY { get; set; }
        public DateTime THOIGIAN_GUI { get; set; }
        public int TRANGTHAI_XOA { get; set; }
    }

}
