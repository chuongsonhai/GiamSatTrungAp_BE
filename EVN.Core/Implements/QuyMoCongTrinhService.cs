using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class QuyMoCongTrinhService : FX.Data.BaseService<QuyMoCongTrinh, int>, IQuyMoCongTrinhService
    {
        public QuyMoCongTrinhService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public QuyMoCongTrinh GetByBienBanDNID(int id)
        {
            var query = Query.Where(x => x.BienBanID == id).FirstOrDefault();
            return query;
        }
    }
}
