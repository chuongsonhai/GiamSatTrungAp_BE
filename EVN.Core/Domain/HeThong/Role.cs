using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Domain
{
    public class Role
    {
        public virtual int groupId { get; set; }
        public virtual string groupName { get; set; }
        public virtual string description { get; set; }
        public virtual bool isSysadmin { get; set; } = true;
        public virtual int status { get; set; }
        public virtual int parentGroupId { get; set; }
        public virtual IList<Permission> Permissions { get; set; } = new List<Permission>();
    }

    public class RoleData
    {
        public RoleData()
        {

        }

        public RoleData(Role role) : base()
        {
            groupName = role.groupName;
            description = role.description;
            isSysadmin = role.isSysadmin;
            Permissions = role.Permissions.Select(p => p.Code).ToList();
        }        
        public string groupName { get; set; }
        public string description { get; set; }
        public bool isSysadmin { get; set; } = true;
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
