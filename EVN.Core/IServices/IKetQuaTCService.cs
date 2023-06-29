using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IKetQuaTCService : FX.Data.IBaseService<KetQuaTC, int>
    {
        KetQuaTC GetbyMaYCau(string maYCau);
        bool SaveKetQua(BienBanDN bienbandn, KetQuaTC item, PhanCongTC phancongtc);
    }
}
