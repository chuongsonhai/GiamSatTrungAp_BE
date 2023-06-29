using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class TNgaiCongViec
    {
        public virtual int ID { get; set; }
        public virtual string MA_TNGAI { get; set; }
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public virtual DateTime NGAY_TAO { get; set; } = DateTime.Now;
        public virtual string NGUOI_TAO { get; set; }
        public virtual string MA_CNANG { get; set; }
    }
}
