using EVN.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace EVN.Core.Domain
{
    public class ThietBi
    {
        public virtual int ID { get; set; }
        public virtual string MaDViQLy { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string Ten { get; set; }
        public virtual decimal CongSuat { get; set; }
        public virtual decimal SoLuong { get; set; }
        public virtual decimal HeSoDongThoi { get; set; }
        public virtual decimal SoGio { get; set; }
        public virtual decimal SoNgay { get; set; }
        public virtual decimal TongCongSuat { get; set; }
        public virtual decimal DienNangSD { get; set; }
        public virtual string MucDichSD { get; set; }
    }
}
