using EVN.Core.Domain;

namespace EVN.Core
{
    public class TienTrinhRequest
    {
        public TienTrinhRequest()
        {

        }
        public TienTrinhRequest(DvTienTrinh tientrinh) : base()
        {
            MA_DVIQLY = tientrinh.MA_DVIQLY;
            MA_YCAU_KNAI = tientrinh.MA_YCAU_KNAI;

            MA_BPHAN_GIAO = tientrinh.MA_BPHAN_GIAO;
            MA_NVIEN_GIAO = tientrinh.MA_NVIEN_GIAO;
            MA_BPHAN_NHAN = tientrinh.MA_BPHAN_NHAN;
            MA_NVIEN_NHAN = tientrinh.MA_NVIEN_NHAN;

            MA_CNANG = tientrinh.MA_CNANG;
            MA_CVIEC = tientrinh.MA_CVIEC;
            MA_CVIECTIEP = tientrinh.MA_CVIECTIEP;

            MA_DDO_DDIEN = tientrinh.MA_DDO_DDIEN;
            MA_TNGAI = tientrinh.MA_TNGAI;

            NDUNG_XLY = tientrinh.NDUNG_XLY;
            NGAY_BDAU = tientrinh.NGAY_BDAU.ToString("dd/MM/yyyy");
            NGAY_HEN = tientrinh.NGAY_HEN.ToString("dd/MM/yyyy");
            NGAY_KTHUC = tientrinh.NGAY_KTHUC.Value.ToString("dd/MM/yyyy");

            NGUOI_SUA = tientrinh.NGUOI_SUA;
            NGUOI_TAO = tientrinh.NGUOI_TAO;

            NGAY_TAO = tientrinh.NGAY_TAO.ToString("dd/MM/yyyy");
            NGAY_SUA = tientrinh.NGAY_SUA.ToString("dd/MM/yyyy");

            NGUYEN_NHAN = tientrinh.NGUYEN_NHAN;
            SO_NGAY_LVIEC = tientrinh.SO_NGAY_LVIEC;
            SO_LAN = tientrinh.SO_LAN;
        }
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_CNANG { get; set; } = "WEB_TBAC_D";
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_CVIECTIEP { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; } //Mã KH
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }
        public virtual string MA_TNGAI { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string NDUNG_XLY { get; set; }

        public virtual string NGAY_BDAU { get; set; }
        public virtual string NGAY_HEN { get; set; }
        public virtual string NGAY_KTHUC { get; set; }

        public virtual string NGUOI_SUA { get; set; }
        public virtual string NGUOI_TAO { get; set; }

        public virtual string NGAY_SUA { get; set; }
        public virtual string NGAY_TAO { get; set; }

        public virtual string NGUYEN_NHAN { get; set; }
        public virtual int SO_LAN { get; set; } = 1;
        public virtual string SO_NGAY_LVIEC { get; set; }
    }
}
