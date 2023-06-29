using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThoaThuanTyLeService : FX.Data.IBaseService<ThoaThuanTyLe, int>
    {
        ThoaThuanTyLe GetbyMaYCau(string maYCau);
        ThoaThuanTyLe GetbyCongvan(int congvanid);
        ThoaThuanTyLe CreateNew(ThoaThuanTyLe thoaThuanTyLe, IList<MucDichThucTeSDD> mucDichs, IList<GiaDienTheoMucDich> giaDiens, out string message);
        ThoaThuanTyLe Update(ThoaThuanTyLe thoaThuanTyLe, IList<MucDichThucTeSDD> mucDichs, IList<GiaDienTheoMucDich> giaDiens, out string message);

        bool Sign(ThoaThuanTyLe item);
    }
}
