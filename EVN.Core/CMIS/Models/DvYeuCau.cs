using EVN.Core.Domain;
using System;

namespace EVN.Core.CMIS
{
    public class DvYeuCau
    {
        public DvYeuCau() { }
        public DvYeuCau(CongVanYeuCau congvan) : base()
        {
            MA_DVIQLY = congvan.MaDViQLy;
            MA_YCAU_KNAI = congvan.MaYeuCau;
            MA_KHANG = congvan.MaKHang ?? string.Empty;
            TEN_NGUOIYCAU = congvan.NguoiYeuCau ?? String.Empty;
            DCHI_NGUOIYCAU = congvan.DChiNguoiYeuCau ?? String.Empty;
            SO_NHA = congvan.SoNha ?? String.Empty;            
            DUONG_PHO = !string.IsNullOrWhiteSpace(congvan.DuongPho) ? congvan.DuongPho : DCHI_NGUOIYCAU;
            NGAY_TNHAN = congvan.NgayDuyet.ToString("dd/MM/yyyy");
            NOI_DUNG_YCAU = congvan.NoiDungYeuCau;
            DTHOAI = congvan.DienThoai;
            NGUOI_TAO = congvan.NguoiLap;
            NGUOI_SUA = congvan.NguoiDuyet;
        }
        public string MA_DVIQLY { get; set; } = string.Empty;
        public string MA_YCAU_KNAI { get; set; } = string.Empty;
        public string MA_KHANG { get; set; } = string.Empty;
        public string TEN_NGUOIYCAU { get; set; } = string.Empty;
        public string DCHI_NGUOIYCAU { get; set; } = string.Empty;
        public string SO_NHA { get; set; } = string.Empty;
        public string DUONG_PHO { get; set; } = string.Empty;
        public string NGAY_TNHAN { get; set; } = string.Empty;
        public string NOI_DUNG_YCAU { get; set; } = string.Empty;
        public string NGAY_HTHANH { get; set; } = string.Empty;
        public string MA_TCHAT { get; set; } = "GD";
        public string MA_HTHUC { get; set; } = "GD";
        public string MA_LOAI_YCAU { get; set; } = "TBAC_D";
        public int TINH_TRANG { get; set; } = 1;
        public string DTHOAI { get; set; } = string.Empty;
        public string SNGAY_YCAU { get; set; } = "0";
        public string SNGAY_ND { get; set; } = "0";
        public string SNGAY_KH { get; set; } = "0";
        public string NGAY_TAO { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string NGUOI_TAO { get; set; } = string.Empty;
        public string NGAY_SUA { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string NGUOI_SUA { get; set; } = string.Empty;
        public string MA_CNANG { get; set; } = "3";
        public string MA_TO { get; set; } = string.Empty;
    }
}
