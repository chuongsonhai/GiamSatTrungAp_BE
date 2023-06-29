using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class SendMail
    {
        public virtual int ID { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string TIEUDE { get; set; }
        public virtual string NOIDUNG { get; set; }
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual DateTime NGAY_TAO { get; set; } = DateTime.Now;
        public virtual DateTime NGAY_GUI { get; set; } = DateTime.Now;
        public virtual string NGUOI_TAO { get; set; }
        public virtual int TRANG_THAI { get; set; } = 0;
    }
}
