using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace EVN.Core.Models
{
    public class ThoiGianCapDienModel
    {

        public virtual string TenDV { get; set; }
        public virtual int TongSoCTTiepNhanTTDN { get; set; }
        public virtual decimal TongSoNgayTB { get; set; } = 0;
        //public virtual decimal SoNgayQuaHan { get; set; } = 0;
        //public virtual decimal SoNgayThucHienTBTTDN { get; set; } = 0;
    }
}
