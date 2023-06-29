using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class BoPhan
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_BPHAN { get; set; }
        public virtual string TEN_BPHAN { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual string GHI_CHU { get; set; }

    }
}