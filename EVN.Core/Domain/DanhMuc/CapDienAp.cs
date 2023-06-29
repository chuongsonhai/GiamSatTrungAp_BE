using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class CapDienAp
    {
        public virtual string MA_CAPDA { get; set; }
        public virtual string TEN_CAPDA { get; set; }
        public virtual string TENTAT_CAPDA { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual int STT_HTHI { get; set; }
        public virtual int TRANG_THAI { get; set; }
    }
}
