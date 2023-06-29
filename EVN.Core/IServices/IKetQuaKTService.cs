using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IKetQuaKTService : FX.Data.IBaseService<KetQuaKT, int>
    {
        KetQuaKT GetbyMaYCau(string maYCau);

        bool SaveKetQua(YCauNghiemThu congvan, KetQuaKT item);
    }
}
