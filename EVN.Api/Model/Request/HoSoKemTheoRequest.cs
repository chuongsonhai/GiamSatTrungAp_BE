using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class HSoKemTheoRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public HSoKemTheoFilter Filter { get; set; }
    }

    public class HSoKemTheoFilter
    {
        public HSoKemTheoFilter()
        {

        }
        public string maDViQly { get; set; }
        public string maYCau { get; set; }
        public string maCongViec { get; set; }
        public int loai { get; set; }
    }

    public class HSoGiayToFilter
    {
        public HSoGiayToFilter()
        {

        }
        public string maDViQly { get; set; }
        public string maYCau { get; set; }
        public string loaiHS { get; set; }
    }


    public class SignDocRequest
    {
        public string maDViQLy { get; set; }
        public string maYCau { get; set; }
        public int[] ids { get; set; }
    }
}
