using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TroNgaiService : FX.Data.BaseService<TroNgai, string>, ITroNgaiService
    {
        public TroNgaiService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<TroNgai> GetbyFilter(string keyword, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MA_TNGAI.Contains(keyword) || p.TEN_TNGAI.Contains(keyword));
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }
    }
}
