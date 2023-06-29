using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class ChiTietDamBao
    {
        public virtual int ID { get; set; }
        public virtual int ThoaThuanID { get; set; }
        public virtual string MucDich { get; set; }
        public virtual decimal SLTrungBinh { get; set; }
        public virtual decimal SoNgayDamBao { get; set; }
        public virtual decimal GiaBanDien { get; set; }
        public virtual decimal ThanhTien { get; set; }
    }
}
