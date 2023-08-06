using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class XacNhanTroNgaiFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public Filter Filter { get; set; }

    }

    public class Filter
    {

        public string tuNgay { get; set; }
        public string denNgay { get; set; }
        public int trangThaiKhaoSat { get; set; } = 0;
        public string maYeuCau { get; set; } 
        public string donViQuanLy { get; set; } = string.Empty;

    }

}
