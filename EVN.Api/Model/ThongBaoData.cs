using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class ThongBaoData
    {
        public ThongBaoData()
        {

        }

        public ThongBaoData(ThongBao item) : base()
        {
            ID = item.ID;
            Loai = item.Loai == LoaiThongBao.KhaoSat ? "Khảo sát" : item.Loai == LoaiThongBao.KiemTra ? "Kiểm tra" : item.Loai == LoaiThongBao.TreoThao ? "Treo tháo" : item.Loai == LoaiThongBao.NghiemThu ? "Nghiệm thu": "Thông báo";
            MaDViQLy = item.MaDViQLy;
            MaYeuCau = item.MaYeuCau;
            NoiDung = item.NoiDung;
            NgayTao = item.NgayTao.ToString("dd/MM/yyyy");
            NguoiTao = item.NguoiTao;
            TrangThai = (int)item.TrangThai;
            if (item.TrangThai != TThaiThongBao.DaXuLy && item.TrangThai != TThaiThongBao.ThongBao)
            {
                if (DateTime.Today >= item.NgayHen)
                    TrangThai = (int)TThaiThongBao.QuaHan;
            }
            BPhanNhan = item.BPhanNhan;
            NguoiNhan = item.NguoiNhan;
            MaCViec = item.MaCViec;
            CongViec = item.CongViec;
            NgayHen = item.NgayHen.ToString("dd/MM/yyyy");
            DuAnDien = item.DuAnDien;
            KhachHang = item.KhachHang;
        }
        public int ID { get; set; }
        public string Loai { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string NoiDung { get; set; }
        public string NgayTao { get; set; }
        public virtual string NguoiTao { get; set; }
        public int TrangThai { get; set; }
        public virtual string BPhanNhan { get; set; }
        public virtual string NguoiNhan { get; set; }
        public virtual string MaCViec { get; set; }
        public virtual string CongViec { get; set; }
        public string NgayHen { get; set; }
        public virtual string DuAnDien { get; set; }
        public virtual string KhachHang { get; set; }
    }

    public class ThongBaoUpdateStatus
    {
        public List<int> tBaoIDs { get; set; } = new List<int>();
    }
}