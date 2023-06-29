using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IPhanCongKSService : FX.Data.IBaseService<PhanCongKS, int>
    {
        PhanCongKS GetbyMaYCau(string loaiYCau, string maYCau);
        bool SavePhanCong(CongVanYeuCau congvan, PhanCongKS item);

        bool Cancel(CongVanYeuCau congvan, PhanCongKS item);
    }
}
