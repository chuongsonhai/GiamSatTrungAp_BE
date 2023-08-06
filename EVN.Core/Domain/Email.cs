using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Email
    {
        public virtual int ID { get; set; }
        public virtual string MA_DVIQLY { get; set; }
        public virtual string MA_KHANG { get; set; }
        public virtual char MA_DVU { get; set; }
        public virtual char EMAIL { get; set; }
        public virtual char NOI_DUNG { get; set; }
        public virtual int TINH_TRANG { get; set; }
        public virtual DateTime NGAY_TAO { get; set; }
        public virtual char NGUOI_TAO { get; set; }
        public virtual DateTime NGAY_SUA { get; set; }
        public virtual char NGUOI_SUA { get; set; }
        public virtual int ID_HDON { get; set; }
        public virtual char TIEU_DE { get; set; }
    }
}
