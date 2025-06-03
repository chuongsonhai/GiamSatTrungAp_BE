using EVN.Core.Domain;
using System;

namespace EVN.Core.CMIS
{
    public class TienTrinh
    {
        public TienTrinh() { }
        public TienTrinh(DvTienTrinh tientrinh): base()
        {
            MA_BPHAN_GIAO = tientrinh.MA_BPHAN_GIAO ?? "";
            MA_BPHAN_NHAN = tientrinh.MA_BPHAN_NHAN;
            MA_CNANG = tientrinh.MA_CNANG;
            MA_CVIEC = tientrinh.MA_CVIEC;
            MA_CVIECTIEP = tientrinh.MA_CVIECTIEP;
            MA_DDO_DDIEN = tientrinh.MA_DDO_DDIEN ?? "1"; 
            MA_DVIQLY = tientrinh.MA_DVIQLY;
            MA_NVIEN_GIAO = tientrinh.MA_NVIEN_GIAO ?? "";
            MA_NVIEN_NHAN = tientrinh.MA_NVIEN_NHAN;
            MA_TNGAI = tientrinh.MA_TNGAI ?? "";
            MA_YCAU_KNAI = tientrinh.MA_YCAU_KNAI;
            NDUNG_XLY = tientrinh.NDUNG_XLY ?? "1";
            NGAY_BDAU = tientrinh.NGAY_BDAU.ToString("yyyy-mm-dd HH:mm:ss");
            NGAY_HEN = tientrinh.NGAY_HEN.ToString("yyyy-mm-dd HH:mm:ss");
            NGAY_KTHUC = tientrinh.NGAY_KTHUC.HasValue ? tientrinh.NGAY_KTHUC.Value.ToString("yyyy-mm-dd HH:mm:ss") : DateTime.Today.ToString("yyyy-mm-dd HH:mm:ss") ;
            NGAY_SUA = tientrinh.NGAY_SUA.ToString("yyyy-mm-dd HH:mm:ss");
            NGAY_TAO = tientrinh.NGAY_TAO.ToString("yyyy-mm-dd HH:mm:ss");
            NGUOI_SUA = tientrinh.NGUOI_SUA ?? "admin";
            NGUOI_TAO = tientrinh.NGUOI_TAO ?? "admin";
            NGUYEN_NHAN = tientrinh.NGUYEN_NHAN ?? "";
            SO_LAN = tientrinh.SO_LAN;
            SO_NGAY_LVIEC = tientrinh.SO_NGAY_LVIEC ?? "";
        }
        public virtual int KQUA_ID_BUOC { get; set; }
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_CNANG { get; set; } = "-1";
        public virtual string MA_CVIEC { get; set; } = string.Empty;
        public virtual string MA_CVIECTIEP { get; set; } = string.Empty;
        public virtual string MA_DDO_DDIEN { get; set; } = "-1";
        public virtual string MA_DVIQLY { get; set; } = string.Empty;
        public virtual string MA_NVIEN_GIAO { get; set; } = string.Empty;
        public virtual string MA_NVIEN_NHAN { get; set; } = string.Empty;
        public virtual string MA_TNGAI { get; set; } = string.Empty;
        public virtual string MA_YCAU_KNAI { get; set; } = string.Empty;
        public virtual string NDUNG_XLY { get; set; } = string.Empty;

        public virtual string NGAY_BDAU { get; set; } = string.Empty;
        public virtual string NGAY_BDAU_HTHI { get; set; } = string.Empty;
        public virtual string NGAY_HEN { get; set; } = string.Empty;
        public virtual string NGAY_KTHUC { get; set; } = string.Empty;
        public virtual string NGAY_KTHUC_HTHI { get; set; } = string.Empty;

        public virtual string NGAY_SUA { get; set; } = string.Empty;
        public virtual string NGAY_TAO { get; set; } = string.Empty;

        public virtual string NGUOI_SUA { get; set; } = string.Empty;
        public virtual string NGUOI_TAO { get; set; } = string.Empty;
        public virtual string NGUYEN_NHAN { get; set; } = string.Empty;
        public virtual int SO_LAN { get; set; } = 1;
        public virtual string SO_NGAY_LVIEC { get; set; } = "1";
        public virtual string WORKFLOW_ID { get; set; } = "0";
    }
}
