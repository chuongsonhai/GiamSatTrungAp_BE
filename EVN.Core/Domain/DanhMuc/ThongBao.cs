using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class ThongBao
    {
        public virtual int ID { get; set; }
        public virtual LoaiThongBao Loai { get; set; } = LoaiThongBao.KhaoSat;
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string NoiDung { get; set; }
        public virtual DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual string NguoiTao { get; set; }
        public virtual TThaiThongBao TrangThai { get; set; } = TThaiThongBao.Moi;
        public virtual string BPhanNhan { get; set; }
        public virtual string NguoiNhan { get; set; }
        public virtual string MaCViec { get; set; }
        public virtual string CongViec { get; set; }
        public virtual DateTime NgayHen { get; set; } = DateTime.Now;
        public virtual string DuAnDien { get; set; }
        public virtual string KhachHang { get; set; }
    }

    public enum LoaiThongBao
    {
        KhaoSat = 0,
        KiemTra = 1,
        TreoThao = 2,
        NghiemThu = 3,
        ThongBao = 10,
        CanhBao = 13
    }

    public enum TThaiThongBao
    {
        Moi = 0,
        DaXuLy = 1,
        QuaHan = 2,
        ThongBao = 7
    }
}
