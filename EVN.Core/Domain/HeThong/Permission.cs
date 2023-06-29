using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Permission
    {
        public virtual int PermissionID { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string GroupCode { get; set; }
    }
}
