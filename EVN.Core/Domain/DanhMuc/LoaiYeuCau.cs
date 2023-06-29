using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class LoaiYeuCau
    {
        public virtual string MA_LOAI { get; set; }
        public virtual string TEN_LOAI { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual int? SO_NGAY { get; set; }
        public virtual string MA_TTO { get; set; }
        public virtual string MA_DTUONG { get; set; }
        public virtual int? STT { get; set; }
        public virtual string MA_NHOM_YCAU { get; set; }
        public virtual bool TRUNG_AP { get; set; } = false;
    }
}
