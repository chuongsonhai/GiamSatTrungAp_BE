using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class RoleService : FX.Data.BaseService<Role, int>, IRoleService
    {
        public RoleService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public Role GetByGroupName(string groupname)
        {
            return Query.Where(x=>x.groupName == groupname).FirstOrDefault();
        }
    }
}
