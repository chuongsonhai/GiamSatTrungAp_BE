using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class BPhanCongViecService : FX.Data.BaseService<BPhanCongViec, int>, IBPhanCongViecService
    {
        public BPhanCongViecService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<BPhanCongViec> GetbyBPhan(string maDViQLy, string maBPhan)
        {
            return Query.Where(p => p.MA_DVIQLY == maDViQLy && p.MA_BPHAN == maBPhan).ToList();
        }

        public IList<BPhanCongViec> GetbyTienTrinh(string maDViQLy, string maLoaiYCau, string[] maCViecs)
        {
            return Query.Where(p => p.MA_DVIQLY == maDViQLy &&  maCViecs.Contains(p.MA_CVIEC)).ToList();
        }
    }
}
