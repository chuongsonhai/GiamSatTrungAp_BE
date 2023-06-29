using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class HopDongModel
    {
        public HopDongModel()
        {

        }
        public HopDongModel(HopDong entity) : base()
        {
            ID = entity.ID;
            MaDViQLy = entity.MaDViQLy;
            MaDViTNhan = entity.MaDViTNhan;
            MaYeuCau = entity.MaYeuCau;

            NgayHopDong = entity.NgayHopDong.ToString("dd/MM/yyyy");
            DonVi = entity.DonVi;
            MaSoThue = entity.MaSoThue;
            DaiDien = entity.DaiDien;
            ChucVu = entity.ChucVu;
            DiaChi = entity.DiaChi;
            DienThoai = entity.DienThoai;
            Email = entity.Email;
            
            KHMa = entity.KHMa;
            KHTen = entity.KHTen;
            KHDaiDien = entity.KHDaiDien;
            KHDiaChi = entity.KHDiaChi;
            KHDienThoai = entity.KHDienThoai;
            KHEmail = entity.KHEmail;
            
            TrangThai = entity.TrangThai;
            Data = entity.Data;
            
            DienSinhHoat = entity.DienSinhHoat;
        }
        public int ID { get; set; }
        public string MaDViQLy { get; set; }
        public string MaDViTNhan { get; set; }
        public string MaYeuCau { get; set; }
        public int CongVanID { get; set; }

        public string NgayHopDong { get; set; }
        public string DonVi { get; set; }
        public string MaSoThue { get; set; }
        public string DaiDien { get; set; }
        public string ChucVu { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        
        public string KHMa { get; set; }
        public string KHTen { get; set; }
        public string KHDaiDien { get; set; }
        public string KHDiaChi { get; set; }
        public string KHDienThoai { get; set; }
        public string KHEmail { get; set; }
        
        public int TrangThai { get; set; }
        public string Data { get; set; }

        public bool DienSinhHoat { get; set; } = false;
    }
}