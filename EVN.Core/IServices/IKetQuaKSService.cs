using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IKetQuaKSService : FX.Data.IBaseService<KetQuaKS, int>
    {
        KetQuaKS GetbyMaYCau(string maYCau);

        bool SaveKetQua(CongVanYeuCau congvan, KetQuaKS item);
    }
}
