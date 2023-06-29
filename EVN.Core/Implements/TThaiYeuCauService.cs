using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TThaiYeuCauService : FX.Data.BaseService<TThaiYeuCau, string>, ITThaiYeuCauService
    {
        public TThaiYeuCauService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public TThaiYeuCau GetbyStatus(int status, int loai = 0)
        {
            return Get(p => p.TTHAI == status && p.LOAI == loai);
        }
    }
}
