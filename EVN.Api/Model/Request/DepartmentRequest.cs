using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Api.Model
{
    public class DepartmentRequest
    {
        public virtual int? ID { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string UserSign { get; set; }
        public virtual string UserManager { get; set; }
        public virtual int? ParentID { get; set; }
        public virtual bool? IsActive { get; set; } = true;
    }
}
