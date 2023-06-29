using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IThoaThuanDNChiTietService : FX.Data.IBaseService<ThoaThuanDNChiTiet, int>
    {
        IList<ThoaThuanDNChiTiet> GetbyYCau(string maYCau);
        ThoaThuanDNChiTiet GetbyType(string maYCau, DoingBusinessType type);
    }
}
