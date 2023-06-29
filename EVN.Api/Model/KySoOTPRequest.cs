using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class KySoOTPRequest
    {
        public string maDonViQuanLy { get; set; }
        public string soDienThoai { get; set; }
    }

    public class XacNhanKySoOTPRequest
    {
        public string sodt { get; set; }
        public string otptoken { get; set; }
        public string maxacnhan { get; set; }
        public string madonvi { get; set; }
        public string nguoiky { get; set; }
        public string tukhoa { get; set; }
        public string base64String { get; set; }
        public string mayeucau { get; set; }

    }

    public class XacNhanBienBanKSRequest
    {
        public string sodt { get; set; }
        public string otptoken { get; set; }
        public string maxacnhan { get; set; }
        public string madonvi { get; set; }
        public string nguoiky { get; set; }
        public string mayeucau { get; set; }
        public string macviec { get; set; }
        public string mabphannhan { get; set; }
        public string manviennhan { get; set; }
    }
}
