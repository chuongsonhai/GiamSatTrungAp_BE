using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class LogCanhBaofilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public LogCanhBaofilter Filter { get; set; }
    }

    public class LogCanhBaofilter
    {
       // public string keyword { get; set; } = string.Empty;
        public string tungay { get; set; } = string.Empty;
        public string denngay { get; set; } = string.Empty;
        public string maLoaiCanhBao { get; set; } = string.Empty;
        public string trangThai { get; set; } = string.Empty;
        public string donViQuanLy { get; set; } = string.Empty;

    }

}
