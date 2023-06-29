using EVN.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Api.Model
{
    public class RoleModel : RoleData
    {
        public RoleModel() { }
        public RoleModel(Role role) : base()
        {
            groupId = role.groupId;
            groupName = role.groupName;
            description = role.description;
            isSysadmin = role.isSysadmin;
            Permissions = role.Permissions.Select(p => p.Code).ToList();
        }
        public int groupId { get; set; } = 0;
    }
}
