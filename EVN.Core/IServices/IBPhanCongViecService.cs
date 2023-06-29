using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IBPhanCongViecService : FX.Data.IBaseService<BPhanCongViec, int>
    {
        IList<BPhanCongViec> GetbyBPhan(string maDViQLy, string maBPhan);
        IList<BPhanCongViec> GetbyTienTrinh(string maDViQLy, string maLoaiYCau, string[] maCViecs);
    }
}
