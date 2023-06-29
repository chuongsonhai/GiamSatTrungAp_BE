using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class HeThongDDChamDutService : FX.Data.BaseService<HeThongDDChamDut, int>, IHeThongDDChamDutService
    {
        public HeThongDDChamDutService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public IList<HeThongDDChamDut> GetByTTDBID(int ttdbid)
        {
            var query = Query.Where(p => p.ThoaThuanID == ttdbid).ToList();
            return query;
        }
    }
}