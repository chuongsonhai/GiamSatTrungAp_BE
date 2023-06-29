using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ChiTietDamBaoService : FX.Data.BaseService<ChiTietDamBao, int>, IChiTietDamBaoService
    {
        public ChiTietDamBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public IList<ChiTietDamBao> GetByTTDBID(int ttdbid)
        {
            var query = Query.Where(p => p.ThoaThuanID == ttdbid).ToList();
            return query;
        }
    }
}