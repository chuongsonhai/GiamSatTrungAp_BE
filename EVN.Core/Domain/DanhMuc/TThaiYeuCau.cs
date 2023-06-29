using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class TThaiYeuCau
    {
        public virtual string MA_TTHAI { get; set; }
        public virtual int TTHAI { get; set; }
        public virtual string TEN_TTHAI { get; set; }
        public virtual string CVIEC_TRUOC { get; set; }
        public virtual string CVIEC { get; set; }
        public virtual int LOAI { get; set; } = 0; //0: Yêu cầu mới, 1: Yêu cầu nghiệm thu, đóng điện
    }
}
