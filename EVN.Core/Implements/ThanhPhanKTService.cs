using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ThanhPhanKTService : FX.Data.BaseService<ThanhPhanKT, int>, IThanhPhanKTService
    {
        public ThanhPhanKTService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public List<ThanhPhanKT> GetByBienBanID(int bienbanid)
        {
            var query = Query.Where(x => x.BienBanID == bienbanid).ToList();
            return query;
        }
    }
}
