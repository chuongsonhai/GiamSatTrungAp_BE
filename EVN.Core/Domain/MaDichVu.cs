using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class MaDichVu
    {
        public virtual string MA_YCAU_KNAI { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string NOI_DUNG_YCAU { get; set; }
        public virtual string TEN_KHANG { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string DTHOAI { get; set; }
        public virtual string ID_WEB { get; set; }
        public virtual int TINH_TRANG { get; set; } = 0;
    }
}
