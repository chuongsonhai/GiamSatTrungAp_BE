using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class BienBanNTModel
    {
        public BienBanNTModel() { }
        public BienBanNTModel(BienBanNT entity) : base()
        {
            ID = entity.ID;
            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;
            ThoaThuanID = entity.ThoaThuanID;
            SoThoaThuan = entity.SoThoaThuan;
            NgayThoaThuan = entity.NgayThoaThuan.ToString("dd/MM/yyyy");
            SoBienBan = entity.SoBienBan;
            NgayLap = entity.NgayLap.ToString("dd/MM/yyyy");
            NguoiLap = entity.NguoiLap;
            TrangThai = entity.TrangThai;
            KHMa = entity.KHMa;
            KHTen = entity.KHTen;
            KHMaSoThue = entity.KHMaSoThue;
            KHDaiDien = entity.KHDaiDien;
            KHChucVu = entity.KHChucVu;
            KHDiaChi = entity.KHDiaChi;
            KHDienThoai = entity.KHDienThoai;
            KHEmail = entity.KHEmail;
            TenCongTrinh = entity.TenCongTrinh;
            DiaDiemXayDung = entity.DiaDiemXayDung;
            Data = entity.Data;
            MaCViec = entity.MaCViec;
        }
        public int ID { get; set; }
        public string MaYeuCau { get; set; }
        public string MaDViQLy { get; set; }
        public int ThoaThuanID { get; set; }
        public string SoThoaThuan { get; set; }
        public string NgayThoaThuan { get; set; }
        public string SoBienBan { get; set; }
        public string NgayLap { get; set; }
        public string NguoiLap { get; set; }
        public int TrangThai { get; set; }
        public string KHMa { get; set; }
        public string KHTen { get; set; }
        public string KHMaSoThue { get; set; }
        public string KHDaiDien { get; set; }
        public string KHChucVu { get; set; }
        public string KHDiaChi { get; set; }
        public string KHDienThoai { get; set; }
        public string KHEmail { get; set; }
        public string TenCongTrinh { get; set; }
        public string DiaDiemXayDung { get; set; }
        public string Data { get; set; }
        public string MaCViec { get; set; }
        public HoSoGiayTo DeNghiNT { get; set; }

        public BienBanNT ToEntity(BienBanNT entity)
        {
            entity.MaYeuCau = MaYeuCau;
            entity.MaDViQLy = MaDViQLy;
            entity.ThoaThuanID = ThoaThuanID;
            entity.SoThoaThuan = SoThoaThuan;
            if (!string.IsNullOrWhiteSpace(NgayThoaThuan))
                entity.NgayThoaThuan = DateTime.ParseExact(NgayThoaThuan, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.SoBienBan = SoBienBan;
            if (!string.IsNullOrWhiteSpace(NgayLap))
                entity.NgayLap = DateTime.ParseExact(NgayLap, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

            entity.NguoiLap = NguoiLap;
            entity.TrangThai = TrangThai;
            entity.KHMa = KHMa;
            entity.KHTen = KHTen;
            entity.KHMaSoThue = KHMaSoThue;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucVu = KHChucVu;
            entity.KHDiaChi = KHDiaChi;
            entity.KHDienThoai = KHDienThoai;
            entity.KHEmail = KHEmail;
            entity.TenCongTrinh = TenCongTrinh;
            entity.DiaDiemXayDung = DiaDiemXayDung;
            entity.Data = Data;
            entity.MaCViec = MaCViec;
            return entity;
        }
    }
}