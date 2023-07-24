using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class KhaoSatFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public KhaoSatFilter Filter { get; set; }
    }

    public class KhaoSatFilter
    {

        public int CANHBAO_ID { get; set; } = 0;
        //public string NOIDUNG_CAUHOI { get; set; } = string.Empty;
        //public string PHANHOI_KH { get; set; } = string.Empty;
        //public DateTime THOIGIAN_KHAOSAT { get; set; } = DateTime.Now;
        //public string NGUOI_KS { get; set; } = string.Empty;
        //public string KETQUA { get; set; } = string.Empty;
        //public string TINHTRANG_KT_CB { get; set; } = string.Empty;
        //public string TRANGTHAI_XOA_KHAOSAT { get; set; } = string.Empty;
        //public string DONVI_QLY { get; set; } = string.Empty;
    }

}
