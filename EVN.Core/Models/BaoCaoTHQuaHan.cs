using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class BaoCaoTHQuaHan
    {
        public virtual string TenDV { get; set; }
        public virtual double SoCTQuaHanTTDN { get; set; }
        public virtual double SoNgayQuaHanTTDN { get; set; }
        public virtual double SoCTQuaHanKTDK { get; set; }
        public virtual double SoNgayQuaHanKTDK { get; set; }
        public virtual double SoCTQuaHanTH { get; set; }
        public virtual double SoNgayQuaHanTH { get; set; }
    }
}
