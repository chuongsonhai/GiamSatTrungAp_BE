using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class CongToService : FX.Data.BaseService<CongTo, int>, ICongToService
    {
        public CongToService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public CongTo GetByBBTTID(int bbid, int loai)
        {
            return Get(x => x.BBAN_ID == bbid && x.LOAI == loai);
        }
    }
}