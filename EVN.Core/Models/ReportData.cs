using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core
{
    public class ReportData
    {
        public double TONG_TGIAN { get; set; }
        public string NGAY_TNHAN { get; set; }
        public string NGAY_KSAT { get; set; }

        public string TEN_NGUOIYCAU { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string NGAY_BDN { get; set; }
        public string NGAY_DT_TDN { get; set; }
        public string NGAY_TTX_TDN { get; set; }
        public double TGIAN_DNOI { get; set; }
        public string NGAY_TH_TVB { get; set; }
        public string NGAY_NV_TVB { get; set; }
        public string NGAY_NT_TVB { get; set; }

        public double TGIAN_NTHU { get; set; }
        public double TGIAN_KSAT { get; set; }
        public double TONG_TGIAN_GIAIQUYET { get; set; }
        public double TGIAN_TNHAN { get; set; }

        public string NGAY_CHUYENKS { get; set; }
        public string NGAY_TNHANYCAU { get; set; }
        public string TEN_DVIQLY { get; set; }
        public string NGAY_HTAT_YC { get; set; }

        public double TGIAN_NDKH { get; set; }
        public string MA_DVIQLY { get; set; }
        public string TRANGTHAIHOSO { get; set; }
    }

    public class BaoCaoTongHopChiTietData
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double SoCTTrongThang { get; set; }
        public double TGTrongThang { get; set; }
        public double SoNgayTrongThang { get; set; }
        public double SoCTLuyKe { get; set; }
        public double TGLuyKe { get; set; }
        public double SoNgayLuyKe { get; set; }

        public double TGTiepNhanTrongThang { get; set; } = 0;

        public double TGKhaoSatTrongThang { get; set; } = 0;
        public int SLKSatTrongThang { get; set; } = 0;

        public double TGTTDNTrongThang { get; set; } = 0;
        public int SLTTDNTrongThang { get; set; } = 0;

        public double TGNTTrongThang { get; set; } = 0;
        public int SLNTTrongThang { get; set; } = 0;

        public double TGTiepNhanLuyKe { get; set; } = 0;

        public double TGKhaoSatLuyKe { get; set; } = 0;
        public int SLKSatLuyKe { get; set; } = 0;

        public double TGTTDNLuyKe { get; set; } = 0;
        public int SLTTDNLuyKe { get; set; } = 0;

        public double TGNTLuyKe { get; set; } = 0;
        public int SLNTLuyKe { get; set; } = 0;

        public double SoNgayTiepNhanTrongThang { get; set; } = 0;
        public double SoNgayKhaoSatTrongThang { get; set; } = 0;
        public double SoNgayTTDNTrongThang { get; set; } = 0;
        public double SoNgayNTTrongThang { get; set; } = 0;
                      
        public double SoNgayTiepNhanLuyKe { get; set; } = 0;
        public double SoNgayKhaoSatLuyKe { get; set; } = 0;
        public double SoNgayTTDNLuyKe { get; set; } = 0;
        public double SoNgayNTLuyKe { get; set; } = 0;

    }
}