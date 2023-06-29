using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class TroNgai
    {
        public virtual string MA_TNGAI { get; set; }
        public virtual string TEN_TNGAI { get; set; }
        public virtual int? LOAI { get; set; }
        public virtual string DTUONG { get; set; }
    }
}
