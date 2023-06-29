using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.CMISApi.Models
{
    public class DvTienTrinhData
    {
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

        public virtual string NGUYEN_NHAN { get; set; }
        public virtual int SO_LAN { get; set; } = 1;
        public virtual string SO_NGAY_LVIEC { get; set; }
    }
}