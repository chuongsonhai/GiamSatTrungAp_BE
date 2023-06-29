using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model.Request
{
    public class YCauNThuRequest
    {
        public string MaDichVu { get; set; }
        public string MaYeuCau { get; set; }
        public string NgayYeuCau { get; set; }
        public string NguoiYeuCau { get; set; }
        public string NoiDungYeuCau { get; set; }
    }
}