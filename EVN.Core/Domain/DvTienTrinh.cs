using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class DvTienTrinh
    {
        [JsonIgnore]
        public virtual long KQ_ID_BUOC { get; set; }
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

        public virtual DateTime NGAY_BDAU { get; set; } = DateTime.Now;
        public virtual DateTime NGAY_HEN { get; set; } = DateTime.Now;
        public virtual DateTime? NGAY_KTHUC { get; set; } 
        public virtual DateTime NGAY_SUA { get; set; } = DateTime.Now;
        public virtual DateTime NGAY_TAO { get; set; } = DateTime.Now;

        public virtual string NGUOI_SUA { get; set; }
        public virtual string NGUOI_TAO { get; set; }
        public virtual string NGUYEN_NHAN { get; set; }
        public virtual int SO_LAN { get; set; } = 1;
        public virtual string SO_NGAY_LVIEC { get; set; }

        [JsonIgnore]
        public virtual int TRANG_THAI { get; set; } = 0;//0: Chưa gửi, 1: Đã gửi, 2: Gửi lỗi, 3: Chờ đồng bộ từ CMIS

        [JsonIgnore]
        public virtual long STT { get; set; } = 0;
    }
}
