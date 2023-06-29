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
    public class GiaDienTheoMucDich
    {
        public virtual int ID { get; set; }
        public virtual int ThoaThuanID { get; set; }
        public virtual string SoCongTo { get; set; }
        public virtual string MaGhiChiSo { get; set; }
        public virtual string ApDungTuChiSo { get; set; }
        public virtual string MucDichSuDung { get; set; }
        public virtual string TyLe { get; set; }
        public virtual string GDKhongTheoTG { get; set; }
        public virtual string GDGioBT { get; set; }
        public virtual string GDGioCD { get; set; }
        public virtual string GDGioTD { get; set; }
    }
}