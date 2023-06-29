using EVN.Core;
using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model.Request
{
    public class YeuCauDataRequest
    {
        public YeuCauDataRequest()
        {
            HoSos = new List<HoSoGiayTo>();
        }
        public YeuCauDataRequest(CongVanYeuCau congVanYeuCau) : base()
        {
            ID = congVanYeuCau.ID;
            MaKHang = congVanYeuCau.MaKHang;
            SoCongVan = congVanYeuCau.SoCongVan;
            MaYeuCau = congVanYeuCau.MaYeuCau;
            NgayYeuCau = congVanYeuCau.NgayYeuCau.ToString("dd-MM-yyyy");
            MaLoaiYeuCau = congVanYeuCau.MaLoaiYeuCau;

            MaDViTNhan = congVanYeuCau.MaDViTNhan;
            MaDViQLy = congVanYeuCau.MaDViQLy;
            BenNhan = congVanYeuCau.BenNhan;

            NguoiYeuCau = congVanYeuCau.NguoiYeuCau;
            DChiNguoiYeuCau = congVanYeuCau.DChiNguoiYeuCau;

            TenKhachHang = congVanYeuCau.TenKhachHang;
            CoQuanChuQuan = congVanYeuCau.CoQuanChuQuan;
            DiaChiCoQuan = congVanYeuCau.DiaChiCoQuan;
            DienThoai = congVanYeuCau.DienThoai;
            Email = congVanYeuCau.Email;
            MST = congVanYeuCau.MST;

            DienSinhHoat = congVanYeuCau.DienSinhHoat;

            NgayHen = congVanYeuCau.NgayHen.HasValue ? congVanYeuCau.NgayHen.Value.ToString("dd/MM/yyyy") : "";
            NgayHenKhaoSat = congVanYeuCau.NgayHenKhaoSat.HasValue ? congVanYeuCau.NgayHenKhaoSat.Value.ToString("dd/MM/yyyy") : "";

            NoiDungYeuCau = congVanYeuCau.NoiDungYeuCau;
            DuAnDien = congVanYeuCau.DuAnDien;
            DiaChiDungDien = congVanYeuCau.DiaChiDungDien;

            TrangThai = (int)congVanYeuCau.TrangThai;
            NgayLap = congVanYeuCau.NgayLap.ToString("dd-MM-yyyy");
            NguoiLap = congVanYeuCau.NguoiLap;
            NguoiDuyet = congVanYeuCau.NguoiDuyet;
            NgayDuyet = congVanYeuCau.NgayDuyet.ToString("dd-MM-yyyy");
            BenNhan = congVanYeuCau.BenNhan;
            MaCViec = congVanYeuCau.MaCViec;
            MaHinhThuc = congVanYeuCau.MaHinhThuc;
            TenKhachHangUQ = congVanYeuCau.TenKhachHangUQ;
            SoDienThoaiKHUQ = congVanYeuCau.SoDienThoaiKHUQ;

            DiaChinhID = congVanYeuCau.DiaChinhID;
            DuongPho = congVanYeuCau.DuongPho;
            SoNha = congVanYeuCau.SoNha;
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

        public string TenKhachHang { get; set; }
        public string CoQuanChuQuan { get; set; }
        public string DiaChiCoQuan { get; set; }
        public string MST { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public bool DienSinhHoat { get; set; }

        public string DuongPho { get; set; }
        public string SoNha { get; set; }
        public string DiaChinhID { get; set; } = "0";

        public string NgayHen { get; set; }
        public string NgayHenKhaoSat { get; set; }

        public string NoiDungYeuCau { get; set; }
        public string DiaChiDungDien { get; set; }
        public string DuAnDien { get; set; }

        public int TrangThai { get; set; }

        public string NgayLap { get; set; }
        public string NguoiLap { get; set; }
        public string NguoiDuyet { get; set; }
        public string NgayDuyet { get; set; }
        public string MaCViec { get; set; }
        public string CongSuat { get; set; }
        public string MaHinhThuc { get; set; }
        public string SoDienThoaiKHUQ { get; set; }
        public string TenKhachHangUQ { get; set; }
        public bool CoHopDong { get; set; } = false;
        public string TroNgai { get; set; }

        public IList<HoSoGiayTo> HoSos { get; set; } = new List<HoSoGiayTo>();

        public CongVanYeuCau ToEntity(CongVanYeuCau entity)
        {
            entity.MaKHang = MaKHang;
            entity.SoCongVan = SoCongVan;
            entity.MaYeuCau = MaYeuCau;
            if (!string.IsNullOrWhiteSpace(NgayYeuCau))
                entity.NgayYeuCau = DateTime.ParseExact(NgayYeuCau, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.MaLoaiYeuCau = MaLoaiYeuCau;
            entity.MaDViTNhan = MaDViTNhan;
            entity.MaDViQLy = MaDViQLy;
            entity.BenNhan = BenNhan;

            entity.NguoiYeuCau = NguoiYeuCau;
            entity.DChiNguoiYeuCau = DChiNguoiYeuCau;
            entity.DienThoai = DienThoai;
            entity.Email = Email;
            entity.DiaChiDungDien = DiaChiDungDien;
            entity.TenKhachHang = TenKhachHang;
            entity.CoQuanChuQuan = CoQuanChuQuan;
            if (string.IsNullOrWhiteSpace(entity.TenKhachHang))
                entity.TenKhachHang = CoQuanChuQuan;
            entity.DiaChiCoQuan = DiaChiCoQuan;
            entity.MST = MST;
            entity.DienThoai = DienThoai;
            entity.Email = Email;
            entity.DienSinhHoat = DienSinhHoat;
            entity.TrangThai = (TrangThaiCongVan)TrangThai;
            if (!string.IsNullOrWhiteSpace(NgayHen))
                entity.NgayHen = DateTime.ParseExact(NgayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            if (!string.IsNullOrWhiteSpace(NgayHenKhaoSat))
                entity.NgayHenKhaoSat = DateTime.ParseExact(NgayHenKhaoSat, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
            entity.NoiDungYeuCau = NoiDungYeuCau;
            entity.DiaChiDungDien = DiaChiDungDien;
            entity.DuAnDien = DuAnDien;

            entity.DuongPho = DuongPho;
            entity.SoNha = SoNha;
            entity.DiaChinhID = DiaChinhID ?? "0";

            entity.SoDienThoaiKHUQ = SoDienThoaiKHUQ;
            entity.TenKhachHangUQ = TenKhachHangUQ;
            return entity;
        }
    }

    public class CongVanYeuCauStatusRequest
    {
        public int ID { get; set; } = 0;
        public string deptId { get; set; }
        public string staffCode { get; set; }
        public string ngayHen { get; set; }
        public string noiDung { get; set; }
    }

    public class YeuCauSignRequest
    {
        public string MaYeuCau { get; set; }
        public string Code { get; set; }
        public string Base64Data { get; set; }
    }
}
