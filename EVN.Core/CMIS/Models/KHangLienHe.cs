using EVN.Core.Domain;
using System;

namespace EVN.Core.CMIS
{
    public class KHangLienHe
    {
        public KHangLienHe()
        {

        }
        public KHangLienHe(CongVanYeuCau congvan) : base()
        {
            MA_DVIQLY = congvan.MaDViQLy;
            MA_YCAU_KNAI = congvan.MaYeuCau;
            MA_KHANG = congvan.MaKHang ?? "";
            TEN_KH_LHE = congvan.NguoiYeuCau;
            DCHI_KH_LHE = congvan.DChiNguoiYeuCau;
            NGAY_TAO = congvan.NgayLap.ToString("dd/MM/yyyy");
            //NGUOI_TAO = congvan.NguoiLap;
            NGUOI_TAO = "ADMIN";
            NGAY_SUA = congvan.NgayDuyet.ToString("dd/MM/yyyy");

            //NGUOI_SUA = congvan.NguoiDuyet;
            NGUOI_SUA = "ADMIN";
            MA_KHANG = congvan.MaKHang;
            DTHOAI_DVU = congvan.DienThoai;
            EMAIL = congvan.Email;
        }
        public string MA_DVIQLY { get; set; } = string.Empty;
        //public string ID_LIENHE { get; set; } = string.Empty;
        public string ID_LIENHE { get; set; } = "0";
        public string MA_HDONG { get; set; } = "0";
        public string MA_YCAU_KNAI { get; set; } = string.Empty;
        public int STTU_UTIEN { get; set; } = 0;
        public string TEN_KH_LHE { get; set; } = string.Empty;
        public string DCHI_KH_LHE { get; set; } = string.Empty;
        public string SO_CMT { get; set; } = "";
        //public string NGAY_CAP { get; set; } = string.Empty;
        public string NGAY_CAP { get; set; } = DateTime.Now.ToString();
        public string NOI_CAP { get; set; } = "HANOI";
        public string NGAY_TAO { get; set; } = DateTime.Now.ToString();
        public string NGUOI_TAO { get; set; } = "ADMIN";
        public string NGAY_SUA { get; set; } = DateTime.Now.ToString();
        public string NGUOI_SUA { get; set; } = "ADMIN";
        public string MA_CNANG { get; set; } = "WEB_TBAC_D";
        public string MA_KHANG { get; set; } = "";
        public string DTHOAI_DVU { get; set; } = string.Empty;
        //public string FAX { get; set; } = string.Empty;
        public string FAX { get; set; } = "0";
        public string EMAIL { get; set; } = string.Empty;
    }
}
