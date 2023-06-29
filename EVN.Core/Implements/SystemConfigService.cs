using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class SystemConfigService : FX.Data.BaseService<SystemConfig, int>, ISystemConfigService
    {
        public SystemConfigService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public SystemConfig GetbyCode(string code)
        {
            return Get(p => p.Code == code);
        }

        public IDictionary<string, string> GetDictionary(string charStart)
        {
            var list = Query.Where(p => p.Code.Contains(charStart)).ToList();
            return list.ToDictionary(x => x.Code, x => x.Value);
        }
    }
}
