using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class LoaiCanhBaoFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public LoaiCanhBaoFilter Filter { get; set; }
    }

    public class LoaiCanhBaoFilter
    {
        public string TenLoaiCanhbao { get; set; } = string.Empty;
        public int maLoaiCanhBao { get; set; } = 0;
    }

}
