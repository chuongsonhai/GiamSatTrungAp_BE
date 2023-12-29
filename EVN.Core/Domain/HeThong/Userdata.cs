using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Userdata
    {
        public virtual int userId { get; set; }
        public virtual string username { get; set; }
        public virtual string fullName { get; set; }
        public virtual string email { get; set; }
        public virtual string phoneNumber { get; set; }
        public virtual int deptId { get; set; }
        public virtual string orgId { get; set; }
        public virtual string staffCode { get; set; }
        public virtual string maDViQLy { get; set; }
        public virtual string maBPhan { get; set; }
        public virtual string maNVien { get; set; }
        public virtual string NotifyId { get; set; }
        public virtual string password { get; set; }
        public virtual string passwordsalt { get; set; }
        public virtual bool isactive { get; set; } = true;
        public virtual IList<Role> Roles { get; set; } = new List<Role>();        
    }
}
