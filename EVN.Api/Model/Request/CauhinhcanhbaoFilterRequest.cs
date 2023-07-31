using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class CauhinhcanhbaoFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public CauhinhcanhbaoFilter Filter { get; set; }
    }

    public class CauhinhcanhbaoFilter
    {
        public int maLoaiCanhBao { get; set; } = 0;
    }

}
