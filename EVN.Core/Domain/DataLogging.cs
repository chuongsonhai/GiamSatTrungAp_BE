using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class DataLogging
    {
        public virtual int ID { get; set; }
        public virtual string MaYeuCau { get; set; }
        public virtual string MaDViQLy { get; set; }

        public virtual DateTime NgayUpdate { get; set; } = DateTime.Now;
        public virtual string UserID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string ComputerIP { get; set; }
        public virtual string ActionType { get; set; }

        public virtual string SourceType { get; set; }

        public virtual string DataLoggingCode { get; set; }

        public virtual string DataLoggingDetail { get; set; }
    }
}
