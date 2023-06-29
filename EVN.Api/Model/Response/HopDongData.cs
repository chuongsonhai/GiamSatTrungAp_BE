using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class HopDongData
    {
        public YeuCauNghiemThuData CongVanYeuCau { get; set; }
        public HopDongModel HopDong { get; set; }
    }
}