using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Api.Model
{
    public class BienBanDNModel
    {
        public BienBanDNModel()
        {

        }
        public BienBanDNModel(BienBanDN entity) : base()
        {
            ID = entity.ID;
            MaKH = entity.MaKH;
            SoBienBan = entity.SoBienBan;
            NgayBienBan = entity.NgayBienBan.ToString("dd/MM/yyyy");

            MaYeuCau = entity.MaYeuCau;
            MaDViQLy = entity.MaDViQLy;

            TenCongTrinh = entity.TenCongTrinh;
            DiaDiemXayDung = entity.DiaDiemXayDung;
            KHTen = entity.KHTen;
            KHDaiDien = entity.KHDaiDien;
            KHChucDanh = entity.KHChucDanh;
            EVNDonVi = entity.EVNDonVi;
            EVNDaiDien = entity.EVNDaiDien;
            EVNChucVu = entity.EVNChucVu;
            EVNDienThoai = entity.EVNDienThoai;
            EVNMaSoThue = entity.EVNMaSoThue;
            EVNDiaChi = entity.EVNDiaChi;

            KHTen = entity.KHTen;
            KHDaiDien = entity.KHDaiDien;
            KHChucDanh = entity.KHChucDanh;
            KHDiaChi = entity.KHDiaChi;
            KHDienThoai = entity.KHDienThoai;
            KHTaiKhoan = entity.KHTaiKhoan;
            KHMaSoThue = entity.KHMaSoThue;
            TenCongTrinh = entity.TenCongTrinh;
            DiaDiemXayDung = entity.DiaDiemXayDung;
            NoiDung = entity.NoiDung;

            NgayDauNoi = entity.NgayDauNoi;
            NgayLap = entity.NgayLap.ToString("dd/MM/yyyy");
            NguoiLap = entity.NguoiLap;
            TrangThai = entity.TrangThai;
            Data = entity.Data;

            TroNgai = entity.TroNgai;
            KHXacNhan = entity.KHXacNhan ? 1 : 0;
        }
        public virtual int ID { get; set; }
        public virtual string MaKH { get; set; }

        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual string SoBienBan { get; set; }
        public string NgayBienBan { get; set; }

        public virtual string EVNDonVi { get; set; }
        public virtual string EVNDaiDien { get; set; }
        public virtual string EVNChucVu { get; set; }
        public virtual string EVNDiaChi { get; set; }
        public virtual string EVNDienThoai { get; set; }
        public virtual string EVNMaSoThue { get; set; }

        public virtual string KHTen { get; set; }
        public virtual string KHDaiDien { get; set; }
        public virtual string KHChucDanh { get; set; }
        public virtual string KHDiaChi { get; set; }
        public virtual string KHDienThoai { get; set; }
        public virtual string KHTaiKhoan { get; set; }
        public virtual string KHMaSoThue { get; set; }
        public virtual string TenCongTrinh { get; set; }
        public virtual string DiaDiemXayDung { get; set; }
        public virtual string NoiDung { get; set; }

        public virtual string NgayDauNoi { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual string NgayLap { get; set; }
        public virtual string NguoiLap { get; set; }
        public int KHXacNhan { get; set; } = 0;
        public string TroNgai { get; set; }
        public virtual string Data { get; set; }
        public bool CusSigned { get; set; } = false;

        public BienBanDN ToEntity(BienBanDN entity)
        {
            entity.MaKH = MaKH;
            entity.SoBienBan = SoBienBan;
            entity.TenCongTrinh = TenCongTrinh;
            entity.DiaDiemXayDung = DiaDiemXayDung;
            entity.KHTen = KHTen;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucDanh = KHChucDanh;
            entity.EVNDonVi = EVNDonVi;
            entity.EVNDiaChi = EVNDiaChi;
            entity.EVNDaiDien = EVNDaiDien;
            entity.EVNChucVu = EVNChucVu;
            entity.EVNDienThoai = EVNDienThoai;
            entity.EVNMaSoThue = EVNMaSoThue;

            entity.KHTen = KHTen;
            entity.KHDaiDien = KHDaiDien;
            entity.KHChucDanh = KHChucDanh;
            entity.KHDiaChi = KHDiaChi;
            entity.KHDienThoai = KHDienThoai;
            entity.KHTaiKhoan = KHTaiKhoan;
            entity.KHMaSoThue = KHMaSoThue;

            entity.TenCongTrinh = TenCongTrinh;
            entity.DiaDiemXayDung = DiaDiemXayDung;
            entity.NoiDung = NoiDung;

            entity.NguoiLap = NguoiLap;
            entity.TrangThai = TrangThai;
            entity.NgayDauNoi = NgayDauNoi;

            if (!string.IsNullOrWhiteSpace(NgayLap))
                entity.NgayBienBan = DateTime.ParseExact(NgayBienBan, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            return entity;
        }
    }       
}