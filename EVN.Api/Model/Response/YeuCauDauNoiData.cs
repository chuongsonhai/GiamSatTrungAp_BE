using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class YeuCauDauNoiData
    {
        public YeuCauDauNoiData()
        {
        }
        public YeuCauDauNoiData(CongVanYeuCau entity) : base()
        {
            ID = entity.ID;
            MaKHang = entity.MaKHang;
            SoCongVan = entity.SoCongVan;
            MaYeuCau = entity.MaYeuCau;
            NgayYeuCau = entity.NgayYeuCau.ToString("dd-MM-yyyy");
            MaLoaiYeuCau = entity.MaLoaiYeuCau;

            MaDViTNhan = entity.MaDViTNhan;
            MaDViQLy = entity.MaDViQLy;
            BenNhan = entity.BenNhan;

            NoiDungYeuCau = entity.NoiDungYeuCau;
            DienSinhHoat = entity.DienSinhHoat;
            NguoiYeuCau = entity.NguoiYeuCau;
            CoQuanChuQuan = entity.CoQuanChuQuan;
            DienThoai = entity.DienThoai;
            Email = entity.Email;
            DiaChiDungDien = entity.DiaChiDungDien;
            DuAnDien = entity.DuAnDien;
            TrangThai = (int)entity.TrangThai;
            NgayLap = entity.NgayLap.ToString("dd-MM-yyyy");
            NguoiLap = entity.NguoiLap;
            NguoiDuyet = entity.NguoiDuyet;
            NgayDuyet = entity.NgayDuyet.ToString("dd-MM-yyyy");
            Data = entity.Data;
            MaCViec = entity.MaCViec;
            LyDoHuy = entity.LyDoHuy;
        }
        public int ID { get; set; }
        public string MaKHang { get; set; }
        public string SoCongVan { get; set; }
        public string MaYeuCau { get; set; }
        public string NgayYeuCau { get; set; }
        public string MaLoaiYeuCau { get; set; }

        public string MaDViTNhan { get; set; }
        public string MaDViQLy { get; set; }
        public string BenNhan { get; set; }

        public string NguoiYeuCau { get; set; }
        public string DChiNguoiYeuCau { get; set; }

        public bool DienSinhHoat { get; set; }

        public string NoiDungYeuCau { get; set; }
        
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChiDungDien { get; set; }
        public string CoQuanChuQuan { get; set; }
        public string DuAnDien { get; set; }
        public int TrangThai { get; set; }
        public string NgayLap { get; set; }
        public string NguoiLap { get; set; }
        public string NguoiDuyet { get; set; }
        public string NgayDuyet { get; set; }
        public string Data { get; set; }
        public string LyDoHuy { get; set; }
        public string MaCViec { get; set; }
        public string TenCViec { get; set; }
        public string MaHinhThuc { get; set; }
    }

    public class YeuCauNghiemThuData
    {
        public YeuCauNghiemThuData()
        {            
        }
        public YeuCauNghiemThuData(YCauNghiemThu entity) : base()
        {
            ID = entity.ID;
            MaKHang = entity.MaKHang;
            SoCongVan = entity.SoCongVan;
            MaYeuCau = entity.MaYeuCau;
            NgayYeuCau = entity.NgayYeuCau.ToString("dd-MM-yyyy");
            MaLoaiYeuCau = entity.MaLoaiYeuCau;

            MaDViQLy = entity.MaDViQLy;

            NoiDungYeuCau = entity.NoiDungYeuCau;
            
            NguoiYeuCau = entity.NguoiYeuCau;
            DiaChi = entity.DiaChi;
            CoQuanChuQuan = entity.CoQuanChuQuan;
            DiaChiCoQuan = entity.DiaChiCoQuan;
            DienThoai = entity.DienThoai;
            Email = entity.Email;
            DiaChiDungDien = entity.DiaChiDungDien;
            DuAnDien = entity.DuAnDien;
            TrangThai = (int)entity.TrangThai;

            SoThoaThuanDN = entity.SoThoaThuanDN;
            Data = entity.Data;
            MaCViec = entity.MaCViec;            
        }
        public virtual int ID { get; set; }
        public virtual string MaKHang { get; set; }
        public virtual string SoCongVan { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string NgayYeuCau { get; set; }
        public virtual string MaLoaiYeuCau { get; set; }

        public virtual string MaDViQLy { get; set; }

        public virtual string NguoiYeuCau { get; set; }
        public virtual string DiaChi { get; set; }

        public virtual string CoQuanChuQuan { get; set; }
        public virtual string DiaChiCoQuan { get; set; }
        public virtual string MaSoThue { get; set; }
        public virtual string DienThoai { get; set; }
        public virtual string Email { get; set; }

        public virtual string SoThoaThuanDN { get; set; }

        public virtual string NoiDungYeuCau { get; set; }

        public virtual string DiaChiDungDien { get; set; }
        public virtual string DuAnDien { get; set; }

        public virtual int TrangThai { get; set; }
        
        public virtual string Fkey { get; set; } = Guid.NewGuid().ToString();
        public virtual string Data { get; set; }
        public virtual string MaCViec { get; set; }
        public string TenCViec { get; set; }
        public string PdfBienBanDN { get; set; }
        public bool GiaoB4 { get; set; } = false;
        public string TroNgai { get; set; }
        public string TrangThaiText { get; set; }
        public string TrangThai_khaosat{ get; set; }
        public string mucdo_hailong { get; set; }

        public string sdt_cmis { get; set; }
        public DateTime NGAY_HTHANH { get; set; }

        public YCauNghiemThu ToEntity(YCauNghiemThu entity)
        {
            entity.MaKHang = MaKHang;
            entity.SoCongVan = SoCongVan;
            entity.MaYeuCau = MaYeuCau;
            if (!string.IsNullOrWhiteSpace(NgayYeuCau))
                entity.NgayYeuCau = DateTime.ParseExact(NgayYeuCau, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.MaLoaiYeuCau = MaLoaiYeuCau;
            entity.MaDViQLy = MaDViQLy;

            entity.NguoiYeuCau = NguoiYeuCau;
            entity.DiaChi = DiaChi;
            entity.DienThoai = DienThoai;
            entity.Email = Email;
            entity.DiaChiDungDien = DiaChiDungDien;
            entity.CoQuanChuQuan = CoQuanChuQuan;
            entity.DiaChiCoQuan = DiaChiCoQuan;
            entity.DienThoai = DienThoai;
            entity.Email = Email;
            entity.SoThoaThuanDN = SoThoaThuanDN;
            entity.TrangThai = (TrangThaiNghiemThu)TrangThai;
            
            entity.NoiDungYeuCau = NoiDungYeuCau;
            entity.DiaChiDungDien = DiaChiDungDien;
            entity.DuAnDien = DuAnDien;

            entity.Data = Data;
            return entity;
        }
    }
}
