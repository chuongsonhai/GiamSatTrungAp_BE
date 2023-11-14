using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class XacNhanTroNgaiFilterkhRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public KhaoSatFilter Filter { get; set; }

    }
    public class KhaoSatFilter
    {

        public string fromdate { get; set; }
        public string todate { get; set; }
        public int maLoaiCanhBao { get; set; } = 0;
        public string maDViQLy { get; set; } = string.Empty;
        public string keyword { get; set; }
        public string trangthai_ycau { get; set; }
        public string trangthai_khaosat { get; set; }
        public string mucdo_hailong { get; set; }
    }

}
