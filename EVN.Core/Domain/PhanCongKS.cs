using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class PhanCongKS
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DDO_DDIEN { get; set; }        
        public virtual string NDUNG_XLY { get; set; }
        
        public virtual string MA_BPHAN_GIAO { get; set; }
        public virtual string MA_NVIEN_GIAO { get; set; }

        public virtual string MA_BPHAN_NHAN { get; set; }
        public virtual string MA_NVIEN_NHAN { get; set; }

        public virtual DateTime NGAY_HEN { get; set; } = DateTime.Now;
        public virtual DateTime NGAY_BDAU { get; set; } = DateTime.Now;
        public virtual DateTime? NGAY_KTHUC { get; set; }

        public virtual string MA_CVIEC_TRUOC { get; set; }
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public virtual string MA_CNANG { get; set; } = "WEB_TBAC_D";

        public virtual int TRANG_THAI { get; set; } = 0;

        public virtual string MA_NVIEN_KS { get; set; }
        public virtual string TEN_NVIEN_KS { get; set; }
        public virtual string DTHOAI_NVIEN_KS { get; set; }
        public virtual string EMAIL_NVIEN_KS { get; set; }
    }
}
