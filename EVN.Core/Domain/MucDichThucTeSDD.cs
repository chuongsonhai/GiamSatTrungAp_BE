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
    public class MucDichThucTeSDD
    {
        public virtual int ID { get; set; }
        public virtual int ThoaThuanID { get; set; }
        public virtual string TenThietBi { get; set; }
        public virtual decimal CongSuat { get; set; }
        public virtual decimal SoLuong { get; set; }
        public virtual decimal HeSoDongThoi { get; set; }
        public virtual decimal SoGio { get; set; }
        public virtual decimal SoNgay { get; set; }
        public virtual decimal TongCongSuatSuDung { get; set; }
        public virtual decimal DienNangSuDung { get; set; }
        public virtual string MucDichSDD { get; set; }
    }
}