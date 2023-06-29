using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class TaiLieuKS
    {
        public virtual int ID { get; set; }
        public virtual int CongVanID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string FileType { get; set; }
        public virtual DateTime CreateDate { get; set; } = DateTime.Now;
        public virtual string Createby { get; set; }
        public virtual int Status { get; set; }
    }
}
