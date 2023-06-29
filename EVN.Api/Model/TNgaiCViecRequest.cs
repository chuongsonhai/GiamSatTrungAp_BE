using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TTrinhRequest
    {
        public string maYCau { get; set; }
        public bool truongBPhan { get; set; } = false;
        public bool nghiemThu { get; set; } = false;
    }
    public class TNgaiCViecRequest
    {
        public string maCViec { get; set; }
        public string maTNgai { get; set; }
    }
}