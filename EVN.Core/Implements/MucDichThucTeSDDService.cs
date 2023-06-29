using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class MucDichThucTeSDDService : FX.Data.BaseService<MucDichThucTeSDD, int>, IMucDichThucTeSDDService
    {
        public MucDichThucTeSDDService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public IList<MucDichThucTeSDD> GetByTTDBID(int ttdbid)
        {
            var query = Query.Where(p => p.ThoaThuanID == ttdbid).ToList();
            return query;
        }
    }
}