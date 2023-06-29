using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ModuleService : FX.Data.BaseService<Module, string>, IModuleService
    {
        public ModuleService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<Module> GetModules(bool? active)
        {
            var query = Query;
            if (active.HasValue) query = query.Where(p => p.IsActive == active.Value);
            return query.OrderBy(p => p.Code).ToList();
        }
    }
}