using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class CauHinhCViec
    {
        public string MaLoaiYCau { get; set; }
        public string MaCViec { get; set; }
        public string TenCongViec { get; set; }
        public string MaCViecTiep { get; set; }
        public string TenCongViecTiep { get; set; }
        public bool CoReNhanh { get; set; } = false;
        public string[] DSMaTroNgai { get; set; } 
    }
}