using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TramBienApService : FX.Data.BaseService<TramBienAp, int>, ITramBienApService
    {
        public TramBienApService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public IList<TramBienAp> getByBienBanDNID(int id)
        {
            var query = Query.Where(x => x.BienBanID == id).ToList();
            return query;
        }
    }
}
