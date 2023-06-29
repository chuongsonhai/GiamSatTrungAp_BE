using EVN.Core.IServices;
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
    public class HeThongDDChamDut
    {
        public virtual int ID { get; set; }
        public virtual int ThoaThuanID { get; set; }
        public virtual string DiemDo { get; set; }
        public virtual string SoCongTo { get; set; }
        public virtual string Loai { get; set; }
        public virtual string TI { get; set; }
        public virtual string TU { get; set; }
        public virtual string HeSoNhan { get; set; }
        public virtual string ChiSoChot { get; set; }
    }
}