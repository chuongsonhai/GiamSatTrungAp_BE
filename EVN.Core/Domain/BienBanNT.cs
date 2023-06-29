using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class BienBanNT
    {
        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual int ThoaThuanID { get; set; }
        public virtual string SoThoaThuan { get; set; }
        public virtual DateTime NgayThoaThuan { get; set; } = DateTime.Now;
        public virtual string SoBienBan { get; set; }
        public virtual DateTime NgayLap { get; set; } = DateTime.Now;
        public virtual string NguoiLap { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual string KHMa { get; set; }
        public virtual string KHTen { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucVu { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHEmail { get; set; }
        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }
        public virtual string Data { get; set; }
        public virtual string MaCViec { get; set; }
    }
}
