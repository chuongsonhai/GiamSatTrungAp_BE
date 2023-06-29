using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IPhanCongTCService : FX.Data.IBaseService<PhanCongTC, int>
    {
        PhanCongTC GetbyMaYCau(string loaiYCau, string maYCau, int loai = 1);
        bool SavePhanCong(BienBanDN bienbandn, PhanCongTC item);

        bool CancelKiemTra(YCauNghiemThu congvan, PhanCongTC item);

        bool CancelThiCong(YCauNghiemThu congvan, PhanCongTC item);
    }
}
