using System.Collections.Generic;

namespace EVN.Core.Domain
{
    public class Module
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; } = true;

        public virtual IList<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
