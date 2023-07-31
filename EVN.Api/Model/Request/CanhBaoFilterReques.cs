using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class CanhBaoFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public CanhBaoFilter Filter { get; set; }
    }

    public class CanhBaoFilter
    {
        public string keyword { get; set; } = string.Empty;
        public int trangThai { get; set; } = 1;
        public int SoLanGui { get; set; } = 1;
        public int maLoaiCanhBao { get; set; } = 1;
        public string fromdate { get; set; } = string.Empty;
        public string todate { get; set; } = string.Empty;
        public string maDViQLy { get; set; } = string.Empty;
    }



}
