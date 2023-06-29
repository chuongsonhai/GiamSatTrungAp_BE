using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class BaoCaoYCNT
    {
        public virtual string MaDViQLy { get; set; }
        public virtual string TenKH { get; set; }
        public virtual string TenCT { get; set; }
        public virtual string DCCT { get; set; }
        public virtual string MaYC { get; set; }
        public virtual decimal TongCS { get; set; }
        public virtual string TongChieuDaiDD { get; set; }
        public virtual double ThoiGianTNHS { get; set; }
        public virtual double ThoiGianTLHS { get; set; }
        public virtual string TroNgaiTNHS { get; set; }
        public virtual double ThoiGianChuyenHSSangKT { get; set; }
        public virtual double SoNgayTNHS { get; set; }
        public virtual double ThoiGianKTThucTe { get; set; }
        public virtual string TroNgaiKT { get; set; }
        public virtual double ThoiGianYeuCauKHHoanThanhTonTai { get; set; }
   
   
        public virtual double ThoiGianLapVaDuyetBBKT { get; set; }
        public virtual double ThoiGianChuyenBBKTDenKH { get; set; }
        public virtual double SoNgayKTDK { get; set; }
        public virtual double ThoiGianTNDNNT { get; set; }
        public virtual string TroNgaiNT { get; set; }
        public virtual double ThoiGianNTDDVaKyHD { get; set; }
        public virtual double SoNgayNTVaKyHD { get; set; }

        public virtual double TongSoNgayYCNT { get; set; }
    }
}
