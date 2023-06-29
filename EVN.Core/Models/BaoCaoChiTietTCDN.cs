using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class BaoCaoChiTietTCDN
    {
        public virtual string MaDViQLy { get; set; }
        public virtual string TenKH { get; set; }
        public virtual string TenCT { get; set; }
        public virtual string DCCT { get; set; }
        public virtual string MaYC { get; set; }
        public virtual decimal TongCS { get; set; }
        public virtual string TongChieuDaiDD { get; set; }
        public virtual string TroNgaiTTDN { get; set; }
        public virtual double TongSoNgayTTDN { get; set; }
        public virtual string TroNgaiKTDK { get; set; }
        public virtual double TongSoNgayKTDK { get; set; }
        public virtual string TroNgaiNT { get; set; }
        public virtual double TongSoNgayNT { get; set; }
        public virtual double TongSoNgayTCDN { get; set; }
        public virtual string HangMucQuaHan { get; set; }
        public virtual double SoNgayQuaHan { get; set; }

    }
}
