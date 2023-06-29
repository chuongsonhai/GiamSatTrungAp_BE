using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class TaiLieuKSService : FX.Data.BaseService<TaiLieuKS, int>, ITaiLieuKSService
    {
        public TaiLieuKSService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<TaiLieuKS> GetbyCongVan(int congvanid)
        {
            return Query.Where(p => p.CongVanID == congvanid).ToList();
        }
    }
}