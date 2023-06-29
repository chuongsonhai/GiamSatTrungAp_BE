using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class CongTo
    {
        public virtual int ID { get; set; }
        public virtual int BBAN_ID { get; set; }
        public virtual string SO_CT { get; set; }
        public virtual string NAMSX_CTO { get; set; }
        public virtual string MAHIEU_CTO { get; set; }
        public virtual string PHA_CTO { get; set; }
        public virtual string LOAI_CTO { get; set; }
        public virtual string TSO_BIENDONG { get; set; }
        public virtual string TSO_BIENAP { get; set; }
        public virtual string NGAY_KDINH { get; set; }
        public virtual string TDIEN_LTRINH { get; set; }
        public virtual string SOLAN_LTRINH { get; set; }
        public virtual string MA_CHIHOP { get; set; }
        public virtual string SO_VIENHOP { get; set; }
        public virtual string MA_CHITEM { get; set; }
        public virtual string SO_VIENTEM { get; set; }
        public virtual string SO_BIEUGIA { get; set; }
        public virtual string CHIEU_DODEM { get; set; }
        public virtual string DO_XA { get; set; }
        public virtual string DONVI_HIENTHI { get; set; }
        public virtual string HSO_MHINH { get; set; }
        public virtual string HSO_HTDODEM { get; set; }
        public virtual string CHI_SO { get; set; }
        public virtual int LOAI { get; set; } = 0; //0: Thao, 1: Treo
    }
}