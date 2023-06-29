using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThoaThuanDamBaoService : FX.Data.IBaseService<ThoaThuanDamBao, int>
    {
        ThoaThuanDamBao GetbyMaYCau(string maYCau);
        ThoaThuanDamBao GetbyCongvan(int congvanid);
        ThoaThuanDamBao CreateNew(ThoaThuanDamBao thoaThuanDamBao, IList<ChiTietDamBao> chiTietDamBaos, out string message);
        ThoaThuanDamBao Update(ThoaThuanDamBao thoaThuanDamBao, IList<ChiTietDamBao> chiTietDamBaos, out string message);

        bool Sign(ThoaThuanDamBao item);
    }
}
