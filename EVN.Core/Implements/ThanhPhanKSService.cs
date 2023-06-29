using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ThanhPhanKSService : FX.Data.BaseService<ThanhPhanKS, int>, IThanhPhanKSService
    {
        public ThanhPhanKSService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public List<ThanhPhanKS> GetByBienBanID(int bienbanid)
        {
            var query = Query.Where(x => x.BienBanID == bienbanid).ToList();
            return query;
        }
    }
}
