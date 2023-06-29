using EVN.Core.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS.Models
{
    public class YeuCauCmisRequest
    {
        public YeuCauCmisRequest(CongVanYeuCau yeucau)
        {
            duAnDien = yeucau.DuAnDien;
            noiDungYeuCau = yeucau.NoiDungYeuCau;
            maKhachHang = String.Empty;
            tenKhachHang = yeucau.TenKhachHang;
            soDienThoaiKhachHang = yeucau.DienThoai;
            emailKhachHang = yeucau.Email;
            quanHuyen = ".";
            phuongXa = ".";
            diaChiGiaoDich = yeucau.DiaChiDungDien;
            diaChiDungDien = yeucau.DiaChiDungDien;
            soChungMinhThu = ".";
            ngayCapCmt = String.Empty;
            noiCapCmt = ".";
            maDonViDiaChinh="";
            tenCoQuan = yeucau.TenKhachHang;
            tenNguoiDaiDien = yeucau.NguoiYeuCau;
            maSoThue = yeucau.MST;
            tenDonViQuanLy = yeucau.MaDViTNhan;
            maDonViQuanLy = yeucau.MaDViQLy;

            int idDiaChinh = 0;
            if (int.TryParse(yeucau.DiaChinhID, out idDiaChinh))
                idDonViDiaChinh = idDiaChinh;

            var ngCapCmt = new DateTime(2000, 1, 1);
            ngayCapCmt = $"{ngCapCmt.ToString("yyyy-MM-dd")}T00:00:00.000Z";
        }
        public string duAnDien { get; set; } = String.Empty;
        public string noiDungYeuCau { get; set; } = String.Empty;
        public string maKhachHang { get; set; } = String.Empty;
        public string tenKhachHang { get; set; } = String.Empty;
        public string soDienThoaiKhachHang { get; set; } = String.Empty;
        public string emailKhachHang { get; set; } = String.Empty;
        public string quanHuyen { get; set; } = String.Empty;
        public string phuongXa { get; set; } = String.Empty;
        public string diaChiGiaoDich { get; set; } = String.Empty;
        public string diaChiDungDien { get; set; } = String.Empty;
        public string soChungMinhThu { get; set; } = String.Empty;
        public string ngayCapCmt { get; set; }
        public string noiCapCmt { get; set; } = String.Empty;
        public string tenCoQuan { get; set; } = String.Empty;
        public string tenNguoiDaiDien { get; set; } = String.Empty;
        public string maSoThue { get; set; } = String.Empty;
        public string tenDonViQuanLy { get; set; } = String.Empty;
        public string maDonViQuanLy { get; set; } = String.Empty;
        public int idDonViDiaChinh { get; set; } = -1;
        public string maDonViDiaChinh { get; set; } = String.Empty;
        public int nguonTiepNhan { get; set; } = 1;
        public string[] fileHoSoGiayTos { get; set; } = new string[] { };
        public string captchaData { get; set; }
    }

    public class YeuCauCmisResult
    {
        public bool isError { get; set; }
        public string message { get; set; }
        public JObject data { get; set; }
    }
}
