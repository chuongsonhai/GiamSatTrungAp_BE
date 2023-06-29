using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class HSKemTheoService : FX.Data.BaseService<HSKemTheo, int>, IHSKemTheoService
    {
        public HSKemTheoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<HSKemTheo> GetbyFilter(string maDViQly, string maYCau, int type)
        {
            return Query.Where(x => x.MaDViQLy == maDViQly && x.MaYeuCau == maYCau && x.Type == type).ToList();
        }

        public HSKemTheo GetbyMaYCau(string maYCau, string loaiHoSo)
        {
            return Get(p => p.MaYeuCau == maYCau && p.LoaiHoSo == loaiHoSo);
        }
    }
}