using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Api.Model
{
    public class TTinYeuCauResponse
    {
        public TTinYeuCauResponse(CongVanYeuCau yeucau)
        {
            MaYeuCau = yeucau.MaYeuCau;
            NguoiYeuCau = yeucau.NguoiYeuCau;
            NgayYeuCau = yeucau.NgayYeuCau.ToString("dd/MM/yyyy");
            DuAnDien = yeucau.DuAnDien;
            DiaChiDungDien = yeucau.DiaChiDungDien;
            MaDDoDDien = yeucau.MaDDoDDien;
            NhuCau = yeucau.NoiDungYeuCau;
            MaLoaiYeuCau = yeucau.MaLoaiYeuCau;
            MaDViQLy = yeucau.MaDViQLy;
            BenNhan = yeucau.BenNhan;
            TenKhachHang = yeucau.TenKhachHang;
            CoQuanChuQuan = yeucau.CoQuanChuQuan;
            DiaChiCoQuan = yeucau.DiaChiCoQuan;

            DienThoai = yeucau.DienThoai;
            Email = yeucau.Email;            
        }
        public string MaYeuCau { get; set; }
        public string NguoiYeuCau { get; set; }
        public string NgayYeuCau { get; set; }
        public string DuAnDien { get; set; }
        public string DiaChiDungDien { get; set; }
        public string MaDDoDDien { get; set; }
        public string NhuCau { get; set; }

        public string MaLoaiYeuCau { get; set; }
        public string MaDViQLy { get; set; }
        public string BenNhan { get; set; }
        public string TenKhachHang { get; set; }
        public string CoQuanChuQuan { get; set; }
        public string DiaChiCoQuan { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }

        public string CongViec { get; set; }

    }
}