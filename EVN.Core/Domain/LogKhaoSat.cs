using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class LogKhaoSat
    {
        public virtual int ID { get; set; }
        public virtual int KHAOSAT_ID { get; set; }
        public virtual int TRANGTHAI { get; set; }
        public virtual string DATA_CU { get; set; }
        public virtual string DATA_MOI { get; set; }
        public virtual DateTime THOIGIAN { get; set; }
        public virtual string NGUOITHUCHIEN { get; set; }

    }
}
