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
        [JsonProperty("filterKH")]
        public KhaoSatFilter FilterKH { get; set; }


    }

    public class KhaoSatFilter
    {

        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        public int maLoaiCanhBao { get; set; } = 0;
        public string donViQuanLy { get; set; } = string.Empty;

    }

}
