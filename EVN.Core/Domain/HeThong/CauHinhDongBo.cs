using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class CauHinhDongBo
    {
        public virtual string MA { get; set; }
        public virtual string MO_TA { get; set; }
        public virtual DateTime NGAY_KTHUC { get; set; } = DateTime.Today;
    }
}
