using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IHoSoGiayToService : FX.Data.IBaseService<HoSoGiayTo, int>
    {
        HoSoGiayTo GetbyCode(string code);
        HoSoGiayTo GetHoSoGiayTo(string maDonVi, string maYCau, string loaiHSo);
        IList<HoSoGiayTo> GetbyYeuCau(string maDonVi, string maYeuCau);

        IList<HoSoGiayTo> ListHSoGTo(string maDonVi, string maYeuCau);

        IList<HoSoGiayTo> ListSign(string maDonVi, string keyword, int pageIndex, int pageSize, out int total);
    }
}
