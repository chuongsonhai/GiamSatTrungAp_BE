using EVN.Core.Domain;
using System;

namespace EVN.Core.CMIS
{
    public class DvTienTNhan
    {
        public DvTienTNhan() { }
        public DvTienTNhan(CongVanYeuCau congvan) : base()
        {
            CQUAN_CHUQUAN = congvan.CoQuanChuQuan;
            DCHI_CQUANCQ = congvan.DiaChiCoQuan;
            DCHI_NGUOIYCAU = congvan.DChiNguoiYeuCau;
            SO_NHA = congvan.SoNha;
            DUONG_PHO = congvan.DuongPho ?? "1";
            DTHOAI = congvan.DienThoai;
            EMAIL = congvan.Email;
            FAX = congvan.Fax ?? "";
            ID_DIA_CHINH = congvan.DiaChinhID;

            MA_DVIQLY = congvan.MaDViQLy;
            MA_KHANG = congvan.MaKHang ?? String.Empty;
            MA_NVIEN = congvan.NguoiDuyet ?? "";
            MA_YCAU_KNAI = congvan.MaYeuCau;
            MDICH_SHOAT = congvan.DienSinhHoat ? "1" : "0";
            MST = congvan.MST;

            NGAY_SUA = congvan.NgayDuyet.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString();
            NGAY_TAO = congvan.NgayLap.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString();
            NGAY_YCAU = congvan.NgayYeuCau.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString();
            NGUOI_SUA = congvan.NguoiDuyet ?? "ADMIN";
            NGUOI_TAO = congvan.NguoiLap ?? "ADMIN";
            NOI_DUNG_YCAU = congvan.NoiDungYeuCau ?? "";
            SO_NHA = congvan.SoNha ?? "";
            SO_PHA = congvan.SoPha;
            SO_TKHOAN = congvan.SoTaiKhoan ?? "";

            TEN_NGUOIYCAU = congvan.NguoiYeuCau;
            MA_DVI_TNHAN = congvan.MaDViTNhan;
            TEN_KHANG = congvan.TenKhachHang;
            if (string.IsNullOrWhiteSpace(congvan.TenKhachHang))
                TEN_KHANG = congvan.NguoiYeuCau;
        }
        public string CQUAN_CHUQUAN { get; set; } = string.Empty;
        public string DCHI_CQUANCQ { get; set; } = string.Empty;
        public string DCHI_NGUOIYCAU { get; set; } = string.Empty;
        public string DTHOAI { get; set; }  = string.Empty;
        public string DTHOAI_DVU { get; set; } = "";

        public string DUONG_PHO { get; set; } = string.Empty;
        public string EMAIL { get; set; } = string.Empty;
        public string FAX { get; set; } = "";
        public string ID_DIA_CHINH { get; set; } = "0";
        public string ID_WEB { get; set; } = "";
        public string LOAI_DINHDANH { get; set; } = "0";
        public string MA_BPHAN { get; set; } = string.Empty;
        public string MA_CNANG { get; set; } = "WEB_TBAC_D";
        public string MA_DVIQLY { get; set; } = string.Empty;
        public string MA_DVI_TNHAN { get; set; } = string.Empty;
        public string MA_HTHUC { get; set; } = "GD";
        public string MA_KHANG { get; set; } = string.Empty;
        public string MA_LOAI_YCAU { get; set; } = "TBAC_D";
        public string MA_NHANG { get; set; } = string.Empty;
        public string MA_NVIEN { get; set; } = string.Empty;
        public string MA_YCAU_KNAI { get; set; } = string.Empty;
        public string MDICH_SHOAT { get; set; } = "0";
        public string MST { get; set; } = string.Empty;
        public string NGAY_HEN { get; set; } = DateTime.Today.ToString();
        public string NGAY_SUA { get; set; } = DateTime.Today.ToString();
        public string NGAY_TAO { get; set; } = DateTime.Today.ToString();
        public string NGAY_UQUYEN { get; set; } = DateTime.Today.ToString();
        public string NGAY_YCAU { get; set; } = DateTime.Today.ToString();

        public string NGUOI_SUA { get; set; } = "admin";
        public string NGUOI_TAO { get; set; } = "admin";
        public string NGUOI_UQUYEN { get; set; } = string.Empty;
        public string NOI_DUNG_YCAU { get; set; } = string.Empty;

        public string SO_NHA { get; set; } = string.Empty;
        public string SO_PHA { get; set; } = "1";
        public int GIOI_TINH { get; set; } = 0;

        public string SO_TKHOAN { get; set; } = string.Empty;
        public string TEN_KHANG { get; set; } = string.Empty;

        public string TEN_NGUOIYCAU { get; set; } = string.Empty;
        public int? TINH_TRANG { get; set; } = 1;
        public string MA_TTO { get; set; } = string.Empty;

        public string MA_HOSO_DVC { get; set; } = "";

        public string TOA_DO { get; set; } = string.Empty;
        public string SO_DINHDANH { get; set; } = string.Empty;
    }
}
