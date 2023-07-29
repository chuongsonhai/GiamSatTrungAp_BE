using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class GiamSatPhanhoiCanhbaoid
    {

        public virtual int idPhanHoi { get; set; }
   
        public virtual string noiDungPhanHoi { get; set; } 
        public virtual string nguoiGui { get; set; }
        public virtual DateTime thoiGianGui { get; set; } = DateTime.Now;
        public virtual int idCanhBao { get; set; }
        public virtual int ID { get; set; }
        public virtual string DONVI_QLY { get; set; }
        public virtual int TRANGTHAI_XOA { get; set; }
        public virtual string FILE_DINHKEM { get; set; }
    }
}
