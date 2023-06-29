using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class LoaiHopDong
    {        
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string ContractType { get; set; }
        public virtual string ServiceType { get; set; }
        public virtual string XmlData { get; set; }
        public virtual string XlstData { get; set; }
    }
}