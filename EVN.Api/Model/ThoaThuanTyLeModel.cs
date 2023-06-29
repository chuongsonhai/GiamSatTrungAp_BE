using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EVN.Api.Model
{
    public class ThoaThuanTyLeModel
    {
        public ThoaThuanTyLeModel()
        {

        }
        public ThoaThuanTyLeModel(ThoaThuanTyLe entity) : base()
        {
            ID = entity.ID;
            CongVanID = entity.CongVanID;
            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;
            Data = entity.Data;
            NgayLap = entity.NgayLap.ToString("dd/MM/yyyy");
            DiaDiem = entity.DiaDiem;
            DonVi = entity.DonVi;
            MaSoThue = entity.MaSoThue;
            SoTaiKhoan = entity.SoTaiKhoan;
            NganHang = entity.NganHang;
            DienThoai = entity.DienThoai;
            Fax = entity.Fax;
            Email = entity.Email;
            DienThoaiCSKH = entity.DienThoaiCSKH;
            Website = entity.Website;
            DaiDien = entity.DaiDien;
            ChucVu = entity.ChucVu;
            SoGiayTo = entity.SoGiayTo;
            NgayCap = entity.NgayCap.ToString("dd/MM/yyyy");
            NoiCap = entity.NoiCap;
            VanBanUQ = entity.VanBanUQ;
            NgayUQ = entity.NgayUQ.ToString("dd/MM/yyyy");
            NguoiKyUQ = entity.NguoiKyUQ;
            NgayKyUQ = entity.NgayKyUQ.ToString("dd/MM/yyyy");
            ChucVuUQ = entity.ChucVuUQ;
            KHTen = entity.KHTen;
            KHMa = entity.KHMa;
            KHDiaChiGiaoDich = entity.KHDiaChiGiaoDich;
            KHDiaChiDungDien = entity.KHDiaChiDungDien;
            KHDangKyKD = entity.KHDangKyKD;
            KHNoiCapDangKyKD = entity.KHNoiCapDangKyKD;
            KHNgayCaoDangKyKD = entity.KHNgayCaoDangKyKD.ToString("dd/MM/yyyy");
            KHMaSoThue = entity.KHMaSoThue;
            KHSoTK = entity.KHSoTK;
            KHNganHang = entity.KHNganHang;
            KHDienThoai = entity.KHDienThoai;
            KHFax = entity.KHFax;
            KHEmail = entity.KHEmail;
            KHDaiDien = entity.KHDaiDien;
            KHChucVu = entity.KHChucVu;
            KHSoGiayTo = entity.KHSoGiayTo;
            KHNgayCap = entity.KHNgayCap.ToString("dd/MM/yyyy");
            KHNoiCap = entity.KHNoiCap;
            KHVanBanUQ = entity.KHVanBanUQ;
            KHNgayUQ = entity.KHNgayUQ.ToString("dd/MM/yyyy");
            KHNguoiKyUQ = entity.KHNguoiKyUQ;
            KHNgayKyUQ = entity.KHNgayKyUQ.ToString("dd/MM/yyyy");
            UngDung = entity.UngDung;
            TrangThai = entity.TrangThai;
            MucDichThucTeSDD = entity.MucDichThucTeSDD;
            GiaDienTheoMucDich = entity.GiaDienTheoMucDich;

    }
        public  int ID { get; set; }
        public  int CongVanID { get; set; }
        public  string MaYeuCau { get; set; }
        public  string MaDViQLy { get; set; }
        public  string Data { get; set; }
        public  string NgayLap { get; set; } 
        public  string DiaDiem { get; set; }
        public  string DonVi { get; set; }
        public  string MaSoThue { get; set; }
        public  string SoTaiKhoan { get; set; }
        public  string NganHang { get; set; }
        public  string DienThoai { get; set; }
        public  string Fax { get; set; }
        public  string Email { get; set; }
        public  string DienThoaiCSKH { get; set; }
        public  string Website { get; set; }
        public  string DaiDien { get; set; }
        public  string ChucVu { get; set; }
        public  string SoGiayTo { get; set; }
        public  string NgayCap { get; set; } 
        public  string NoiCap { get; set; }
        public  string VanBanUQ { get; set; }
        public  string NgayUQ { get; set; } 
        public  string NguoiKyUQ { get; set; }
        public  string NgayKyUQ { get; set; } 
        public  string ChucVuUQ { get; set; }
        public  string KHTen { get; set; }
        public  string KHMa { get; set; }
        public  string KHDiaChiGiaoDich { get; set; }
        public  string KHDiaChiDungDien { get; set; }
        public  string KHDangKyKD { get; set; }
        public  string KHNoiCapDangKyKD { get; set; }
        public  string KHNgayCaoDangKyKD { get; set; } 
        public  string KHMaSoThue { get; set; }
        public  string KHSoTK { get; set; }
        public  string KHNganHang { get; set; }
        public  string KHDienThoai { get; set; }
        public  string KHFax { get; set; }
        public  string KHEmail { get; set; }
        public  string KHDaiDien { get; set; }
        public  string KHChucVu { get; set; }
        public  string KHSoGiayTo { get; set; }
        public  string KHNgayCap { get; set; } 
        public  string KHNoiCap { get; set; }
        public  string KHVanBanUQ { get; set; }
        public  string KHNgayUQ { get; set; } 
        public  string KHNguoiKyUQ { get; set; }
        public  string KHNgayKyUQ { get; set; } 
        public  string UngDung { get; set; }
        public  int TrangThai { get; set; }
        public  IList<MucDichThucTeSDD> MucDichThucTeSDD { get; set; } = new List<MucDichThucTeSDD>();
        public  IList<GiaDienTheoMucDich> GiaDienTheoMucDich { get; set; } = new List<GiaDienTheoMucDich>();

        public ThoaThuanTyLe ToEntity(ThoaThuanTyLe entity)
        {

            entity.ID = ID;
            entity.CongVanID = CongVanID;
            entity.MaYeuCau = MaYeuCau;
            entity.MaDViQLy = MaDViQLy;
            entity.Data = Data;
            if (!string.IsNullOrWhiteSpace(NgayLap))
                entity.NgayLap = DateTime.ParseExact(NgayLap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.DiaDiem = DiaDiem;
            entity.DonVi = DonVi;
            entity.MaSoThue = MaSoThue;
            entity.SoTaiKhoan = SoTaiKhoan;
            entity.NganHang = NganHang;
            entity.DienThoai = DienThoai;
            entity.Fax = Fax;
            entity.Email = Email;
            entity.DienThoaiCSKH = DienThoaiCSKH;
            entity.Website = Website;
            entity.DaiDien = DaiDien;
            entity.ChucVu = ChucVu;
            entity.SoGiayTo = SoGiayTo;
            if (!string.IsNullOrWhiteSpace(NgayCap))
                entity.NgayCap = DateTime.ParseExact(NgayCap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.NoiCap = NoiCap;
            entity.VanBanUQ = VanBanUQ;
            if (!string.IsNullOrWhiteSpace(NgayUQ))
                entity.NgayUQ = DateTime.ParseExact(NgayUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.NguoiKyUQ = NguoiKyUQ;
            if (!string.IsNullOrWhiteSpace(NgayKyUQ))
                entity.NgayKyUQ = DateTime.ParseExact(NgayKyUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.ChucVuUQ = ChucVuUQ;
            entity.KHTen = KHTen;
            entity.KHMa = KHMa;
            entity.KHDiaChiGiaoDich = KHDiaChiGiaoDich;
            entity.KHDiaChiDungDien = KHDiaChiDungDien;
            entity.KHDangKyKD = KHDangKyKD;
            entity.KHNoiCapDangKyKD = KHNoiCapDangKyKD;
            if (!string.IsNullOrWhiteSpace(KHNgayCaoDangKyKD))
                entity.KHNgayCaoDangKyKD = DateTime.ParseExact(KHNgayCaoDangKyKD, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.KHMaSoThue = KHMaSoThue;
            entity.KHSoTK = KHSoTK;
            entity.KHNganHang = KHNganHang;
            entity.KHDienThoai = KHDienThoai;
            entity.KHFax = KHFax;
            entity.KHEmail = KHEmail;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucVu = KHChucVu;
            entity.KHSoGiayTo = KHSoGiayTo;
            if (!string.IsNullOrWhiteSpace(KHNgayCap))
                entity.KHNgayCap = DateTime.ParseExact(KHNgayCap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.KHNoiCap = KHNoiCap;
            entity.KHVanBanUQ = KHVanBanUQ;
            if (!string.IsNullOrWhiteSpace(KHNgayUQ))
                entity.KHNgayUQ = DateTime.ParseExact(KHNgayUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.KHNguoiKyUQ = KHNguoiKyUQ;
            if (!string.IsNullOrWhiteSpace(KHNgayKyUQ))
                entity.KHNgayKyUQ = DateTime.ParseExact(KHNgayKyUQ, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.UngDung = UngDung;
            entity.TrangThai = TrangThai;
            return entity;
        }
    }
}