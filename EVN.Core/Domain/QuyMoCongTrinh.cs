using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class QuyMoCongTrinh
    {
        public virtual int ID { get; set; }
        public virtual int BienBanID { get; set; }
        public virtual int BBKhaoSatID { get; set; }

        public virtual string DiemDau { get; set; }
        public virtual string DiemCuoi { get; set; }
        public virtual string CapDienDauNoi { get; set; }
        public virtual string DayDan { get; set; }
        public virtual string SoMach { get; set; }
        public virtual string ChieuDaiTuyen { get; set; }
        public virtual string KetCau { get; set; }
        public virtual string CheDoVanHanh { get; set; }
        public virtual string MoTaCongTrinh { get; set; }
    }
}
