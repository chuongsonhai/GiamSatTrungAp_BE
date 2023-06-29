using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class BaoCaoTHTCDN
    {
        public virtual string TenDV { get; set; }
        public virtual int TongSoCTTiepNhanTTDN { get; set; }
        public virtual int TongSoCTCoTroNgaiTTDN { get; set; }
        public virtual int TongSoCTDaHoanThanhTTDN { get; set; }
        public virtual int TongSoCTChuaHoanThanhTTDN { get; set; }
        public virtual decimal SoNgayThucHienTBTTDN { get; set; } = 0;
        public virtual int TongSoCTQuaHanTTDN { get; set; }
        public virtual decimal SoNgayQuaHanTTDN { get; set; } = 0;


        public virtual int TongSoCTTiepNhanKTDK { get; set; }
        public virtual int TongSoCTCoTroNgaiKTDK { get; set; }
        public virtual int TongSoCTDaHoanThanhKTDK { get; set; }
        public virtual int TongSoCTChuaHoanThanhKTDK { get; set; }
        public virtual decimal SoNgayThucHienTBKTDK { get; set; } = 0;
        public virtual int TongSoCTQuaHanKTDK { get; set; }
        public virtual decimal SoNgayQuaHanKTDK { get; set; } = 0;

        public virtual int TongSoCTTiepNhanNT { get; set; }
        public virtual int TongSoCTCoTroNgaiNT { get; set; }
        public virtual int TongSoCTDaHoanThanhNT { get; set; }
        public virtual int TongSoCTChuaHoanThanhNT { get; set; }
        public virtual decimal SoNgayThucHienTBNT { get; set; } = 0;
        public virtual int TongSoCTQuaHanNT { get; set; }
        public virtual decimal SoNgayQuaHanNT { get; set; } = 0;

        public virtual decimal TongSoNgayTB { get; set; } = 0;
        public virtual int TongSoCTQuaHan { get; set; }
        public virtual decimal SoNgayQuaHan { get; set; } = 0;
    }
}
