using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class CauHinhCanhBaoLogFilterRequest : BaseRequest
    {

        [JsonProperty("filter")]
        public CauHinhCanhBaoLogRequest Filter{ get; set; }
    }

    public class CauHinhCanhBaoLogRequest
    {

        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int maLoaiCanhBao { get; set; }
        public int trangThai { get; set; }
        public string donViQuanLy { get; set; }
        public int canhBaoId { get; set; }

    }

}
