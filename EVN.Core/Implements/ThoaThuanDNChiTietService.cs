using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class ThoaThuanDNChiTietService : FX.Data.BaseService<ThoaThuanDNChiTiet, int>, IThoaThuanDNChiTietService
    {
        public ThoaThuanDNChiTietService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<ThoaThuanDNChiTiet> GetbyYCau(string maYCau)
        {
            return Query.Where(p => p.MaYeuCau == maYCau).ToList();
        }

        public ThoaThuanDNChiTiet GetbyType(string maYCau, DoingBusinessType type)
        {
            return Get(p => p.MaYeuCau == maYCau && p.Type == type);
        }
    }
}
