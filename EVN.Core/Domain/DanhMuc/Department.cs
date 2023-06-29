using System;

namespace EVN.Core.Domain
{
    public class Department
    {
        public virtual long deptId { get; set; }
        public virtual long orgId { get; set; }
        public virtual string shortName { get; set; }
        public virtual string name { get; set; }
        public virtual int status { get; set; }

        public virtual long? deptParent { get; set; }
        public virtual long? deptRoot { get; set; }
        public virtual DateTime updatetime { get; set; } = DateTime.Now;
    }
}
