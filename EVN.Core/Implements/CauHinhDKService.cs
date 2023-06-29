using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class CauHinhDKService : FX.Data.BaseService<CauHinhDK, int>, ICauHinhDKService
    {
        public CauHinhDKService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<CauHinhDK> GetbyMaCViec(string maLoaiYCau, string maCViec)
        {
            return Query.Where(p => p.MA_CVIEC_TRUOC == maCViec).OrderBy(p => p.ORDERNUMBER).ToList();
        }
    }
}
