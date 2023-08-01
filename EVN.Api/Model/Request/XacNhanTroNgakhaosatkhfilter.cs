using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{

    public class XacNhanTroNgakhaosatkhfilter
    {
        public XacNhanTroNgakhaosatkhfilter()
        {
        }
        public XacNhanTroNgakhaosatkhfilter(Xacnhantrongaikhaosatfilter lKhaoSat1) : base()
        {
            maYeuCau = lKhaoSat1.MaYeuCau;
            tenKhachHang = lKhaoSat1.TenKhachHang;
            trangThaiYeuCau = lKhaoSat1.TrangThai;
        }


        public string maYeuCau { get; set; }
        public string tenKhachHang { get; set; }
        public int trangThaiYeuCau { get; set; }

    }
}
