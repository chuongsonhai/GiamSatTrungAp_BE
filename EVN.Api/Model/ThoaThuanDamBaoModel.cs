using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EVN.Api.Model
{
    public class ThoaThuanDamBaoModel
    {
        public ThoaThuanDamBaoModel()
        {

        }
        public ThoaThuanDamBaoModel(ThoaThuanDamBao entity) : base()
        {
            ID = entity.ID;
            CongVanID = entity.CongVanID;
            Gio = entity.Gio;
            Phut = entity.Phut;
            NgayLap = entity.NgayLap.ToString("dd/MM/yyyy");
            DiaDiem = entity.DiaDiem;

            DonVi = entity.DonVi;
            MaSoThue = entity.MaSoThue;
            DaiDien = entity.DaiDien;
            ChucVu = entity.ChucVu;
            VanBanUQ = entity.VanBanUQ;
            NgayUQ = entity.NgayUQ.ToString("dd/MM/yyyy");
            NguoiKyUQ = entity.NguoiKyUQ;
            NgayKyUQ = entity.NgayKyUQ.ToString("dd/MM/yyyy");
            ChucVuUQ = entity.ChucVuUQ;
            DiaChi = entity.DiaChi;
            DienThoai = entity.DienThoai;
            Email = entity.Email;
            DienThoaiCSKH = entity.DienThoaiCSKH;
            SoTaiKhoan = entity.SoTaiKhoan;
            NganHang = entity.NganHang;
            KHMaSoThue = entity.KHMaSoThue;
            KHChucVu = entity.KHChucVu;
            KHMa = entity.KHMa;
            KHTen = entity.KHTen;
            KHDaiDien = entity.KHDaiDien;
            KHDiaChi = entity.KHDiaChi;
            KHDienThoai = entity.KHDienThoai;
            KHEmail = entity.KHEmail;
            KHSoTK = entity.KHSoTK;
            KHNganHang = entity.KHNganHang;
            KHVanBanUQ = entity.KHVanBanUQ;
            KHNguoiUQ = entity.KHNguoiUQ;
            KHSoGiayTo = entity.KHSoGiayTo;
            NgayCap=entity.NgayCap.ToString("dd/MM/yyyy");
            NoiCap = entity.NoiCap;
            KHNgayUQ = entity.KHNgayUQ.ToString("dd/MM/yyyy");

            GiaTriTien = entity.GiaTriTien;
            TienBangChu = entity.TienBangChu;

            TuNgay = entity.TuNgay.ToString("dd/MM/yyyy");
            DenNgay = entity.DenNgay.ToString("dd/MM/yyyy");

            HinhThuc = entity.HinhThuc;
            GhiChu = entity.GhiChu;
            TrangThai = entity.TrangThai;

            Data = entity.Data;
            GiaTriDamBao = entity.GiaTriDamBao;

        }

        public int ID { get; set; }
        public int CongVanID { get; set; }
        public int Gio { get; set; }
        public int Phut { get; set; }
        public string NgayLap { get; set; }
        public string DiaDiem { get; set; }

        public string DonVi { get; set; }
        public string MaSoThue { get; set; }
        public string DaiDien { get; set; }
        public string ChucVu { get; set; }
        public string VanBanUQ { get; set; }
        public string NgayUQ { get; set; }
        public string NguoiKyUQ { get; set; }
        public string NgayKyUQ { get; set; }
        public string ChucVuUQ { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string DienThoaiCSKH { get; set; }
        public string SoTaiKhoan { get; set; }
        public string NganHang { get; set; }
        public string KHMa { get; set; }
        public string KHTen { get; set; }
        public string KHDaiDien { get; set; }
        public string KHDiaChi { get; set; }
        public string KHDienThoai { get; set; }
        public string KHEmail { get; set; }
        public string KHSoTK { get; set; }
        public string KHNganHang { get; set; }
        public string KHVanBanUQ { get; set; }
        public string KHNguoiUQ { get; set; }
        public string KHNgayUQ { get; set; }
        public string KHMaSoThue { get; set; }
        public string KHDangKyKD { get; set; }
        public string KHSoGiayTo { get; set; }
        public string KHChucVu { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public decimal GiaTriTien { get; set; }
        public string TienBangChu { get; set; }

        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

        public string HinhThuc { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }

        public string Data { get; set; }
        public  IList<ChiTietDamBao> GiaTriDamBao { get; set; } = new List<ChiTietDamBao>();
        public ThoaThuanDamBao ToEntity(ThoaThuanDamBao entity)
        {

            entity.CongVanID = CongVanID;
            entity.Gio = Gio;
            entity.Phut = Phut;
            if (!string.IsNullOrWhiteSpace(NgayLap))
                entity.NgayLap = DateTime.ParseExact(NgayLap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.DiaDiem = DiaDiem;

            entity.DonVi = DonVi;
            entity.MaSoThue = MaSoThue;
            entity.DaiDien = DaiDien;
            entity.ChucVu = ChucVu;
            entity.VanBanUQ = VanBanUQ;
            if (!string.IsNullOrWhiteSpace(NgayUQ))
                entity.NgayUQ = DateTime.ParseExact(NgayUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.NguoiKyUQ = NguoiKyUQ;
            if (!string.IsNullOrWhiteSpace(NgayKyUQ))
                entity.NgayKyUQ = DateTime.ParseExact(NgayKyUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.ChucVuUQ = ChucVuUQ;
            entity.DiaChi = DiaChi;

            entity.DiaDiem = DiaDiem;
            entity.MaSoThue = MaSoThue;
            entity.KHMaSoThue = KHMaSoThue;
            entity.KHSoGiayTo = KHSoGiayTo;
            entity.KHChucVu = KHChucVu;
            entity.KHDangKyKD = KHDangKyKD;
            entity.NoiCap = NoiCap;
            if (!string.IsNullOrWhiteSpace(NgayCap))
                entity.NgayCap = DateTime.ParseExact(NgayCap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.DienThoai = DienThoai;
            entity.Email = Email;
            entity.DienThoaiCSKH = DienThoaiCSKH;
            entity.SoTaiKhoan = SoTaiKhoan;
            entity.NganHang = NganHang;
            entity.KHMa = KHMa;
            entity.KHTen = KHTen;
            entity.KHDaiDien = KHDaiDien;
            entity.KHDiaChi = KHDiaChi;
            entity.KHDienThoai = KHDienThoai;
            entity.KHEmail = KHEmail;
            entity.KHSoTK = KHSoTK;
            entity.KHNganHang = KHNganHang;
            entity.KHVanBanUQ = KHVanBanUQ;
            entity.KHNguoiUQ = KHNguoiUQ;
            if (!string.IsNullOrWhiteSpace(KHNgayUQ))
                entity.KHNgayUQ = DateTime.ParseExact(KHNgayUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.GiaTriTien = GiaTriTien;
            entity.TienBangChu = TienBangChu;
            if (!string.IsNullOrWhiteSpace(TuNgay))
                entity.TuNgay = DateTime.ParseExact(TuNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(DenNgay))
                entity.DenNgay = DateTime.ParseExact(DenNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.HinhThuc = HinhThuc;
            entity.GhiChu = GhiChu;
            entity.TrangThai = TrangThai;
            entity.Data = Data;
            return entity;
        }
    }
}