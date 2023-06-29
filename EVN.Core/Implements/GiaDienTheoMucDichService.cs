using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class GiaDienTheoMucDichService : FX.Data.BaseService<GiaDienTheoMucDich, int>, IGiaDienTheoMucDichService
    {
        public GiaDienTheoMucDichService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public IList<GiaDienTheoMucDich> GetByTTDBID(int ttdbid)
        {
            var query = Query.Where(p => p.ThoaThuanID == ttdbid).ToList();
            return query;
        }
    }
}