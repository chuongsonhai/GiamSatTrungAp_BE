using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TNgaiCongViecService : FX.Data.BaseService<TNgaiCongViec, int>, ITNgaiCongViecService
    {
        public TNgaiCongViecService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<TNgaiCongViec> GetbyTroNgai(string maTNgai)
        {
            return Query.Where(p => p.MA_TNGAI == maTNgai).ToList(); ;
        }
    }
}
