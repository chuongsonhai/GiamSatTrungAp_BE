using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class CongViec
    {
        public virtual string MA_CVIEC { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public virtual string TEN_CVIEC { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual int STT { get; set; }
    }
}
