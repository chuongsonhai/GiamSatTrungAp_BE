using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class CauHinhDK
    {
        public virtual int ID { get; set; }
        public virtual string MA_LOAI_YCAU { get; set; }
        public virtual int WORKFLOW_ID { get; set; }
        public virtual string MA_CVIEC_TRUOC { get; set; }
        public virtual string MA_CVIEC_TIEP { get; set; }        
        public virtual DateTime NGAY_HLUC { get; set; }
        public virtual int? TRANG_THAI_TIEP { get; set; }
        public virtual int TCHAT_KPHUC { get; set; }
        public virtual int ORDERNUMBER { get; set; }
        public virtual int? TTHAI_YCAU { get; set; }
        public virtual LoaiCauHinh LOAI { get; set; } = LoaiCauHinh.ThoaThuanDN;
        public virtual bool TRO_NGAI { get; set; } = false;
    }

    public enum LoaiCauHinh
    {
        ThoaThuanDN = 0,
        NghiemThu = 1
    }
}
