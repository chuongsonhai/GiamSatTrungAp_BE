using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class BaoCaoTTDN
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
        public virtual double ThoiGianChuyenHSSangKS { get; set; }
        public virtual double SoNgayTNHS { get; set; }
        public virtual double ThoiGianDuKienKS { get; set; }
        public virtual string TroNgaiKS { get; set; }
        public virtual double ThoiGianKSThucTe { get; set; }
        public virtual double SoNgayThucHienKS { get; set; }
        public virtual double ThoiGianLapVaDuyetTTDN { get; set; }
        public virtual double SoNgayLapTTDN { get; set; }
        public virtual double ThoiGianChuyenBBTTDNDenKH { get; set; }
        public virtual double SoNgayLapBBTTDN { get; set; }

        public virtual double TongSoNgayTTDN { get; set; }
    }
}
