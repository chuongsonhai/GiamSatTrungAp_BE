using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IBienBanNTService : FX.Data.IBaseService<BienBanNT, int>
    {
        bool Approve(BienBanNT item, out string message);
        BienBanNT GetbyMaYeuCau(string mayeucau);
        IList<BienBanNT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
    }
}
