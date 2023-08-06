using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class DanhMucLoaiCanhBao
    {
        public virtual int ID { get; set; }
        public virtual string TENLOAICANHBAO { get; set; }
        public virtual int CHUKYCANHBAO { get; set; }
        public virtual DateTime THOIGIANCHAYCUOI { get; set; }
        public virtual int TRANGTHAI { get; set; }
    }
}
