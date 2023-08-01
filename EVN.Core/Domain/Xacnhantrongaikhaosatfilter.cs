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
    public class Xacnhantrongaikhaosatfilter
    {
        public virtual string MaYeuCau { get; set; }
        public virtual string TenKhachHang { get; set; }
        public virtual int TrangThai { get; set; }
        public virtual int ID { get; set; }

    }    
}