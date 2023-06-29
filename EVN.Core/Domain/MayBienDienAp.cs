using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class MayBienDienAp
    {
        public virtual int ID { get; set; }
        public virtual int BBAN_ID { get; set; }
        public virtual string SO_TBI { get; set; }
        public virtual string NAM_SX { get; set; }
        public virtual string NGAY_KDINH { get; set; }
        public virtual string LOAI { get; set; }
        public virtual string TYSO_TU { get; set; }
        public virtual string CHIHOP_VIEN { get; set; }
        public virtual string TEM_KD_VIEN { get; set; }
        public virtual bool TU_THAO { get; set; } = true;
    }
}
