using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ThietBiService : FX.Data.BaseService<ThietBi, int>, IThietBiService
    {
        public ThietBiService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {

        }
        public IList<ThietBi> GetbyMaYCau(string maYCau)
        {
            return Query.Where(p => p.MaYeuCau == maYCau).ToList();
        }
        public IList<ThietBi> GetByFilter(string maDonViQL,string maYCau)
        {
            return Query.Where(p => p.MaYeuCau == maYCau && p.MaDViQLy==maDonViQL).ToList();
        }
    }
}