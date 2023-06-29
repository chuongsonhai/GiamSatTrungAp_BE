using EVN.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
  
    public class YeuCauFilterRequest : BaseRequest
    {
        [JsonProperty("filter")]
        public YeuCauFilter Filter { get; set; }
    }

    public class YeuCauFilter
    {
        public string maDViQLy { get; set; }
        public string keyword { get; set; } = string.Empty;
        public string khachhang { get; set; } = string.Empty;
        public int status { get; set; } = 0;
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class SendMailFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public SendMailFilter Filter { get; set; }
    }

    public class SendMailFilter
    {
        public string maYCau { get; set; }
        public string keyword { get; set; }
        public int status { get; set; } = -1;
        public string fromdate { get; set; }
        public string todate { get; set; }
    }


    public class MailCanhBaoTCTFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public MailCanhBaoTCTFilter Filter { get; set; }
    }
    public class MailCanhBaoTCTFilter
    {
        public string tenNV { get; set; }
        public string email { get; set; }
    }

    public class DataLoggingFilterRequest : BaseRequest
    {
        [JsonProperty("Filter")]
        public DataLoggingFilter Filter { get; set; }
    }

    public class DataLoggingFilter
    {
        public string maYCau { get; set; }
        public string maDonVi { get; set; }
        public string userName { get; set; }
        public string keyword { get; set; }
        public int status { get; set; } = -1;
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class TTYeuCauRequest
    {
        public string maDViQLy { get; set; }
        public string maYCau { get; set; }
    }

    public class YeuCauCViecRequest
    {        
        public string tuNgay { get; set; }
        public string denNgay { get; set; }
    }
}
