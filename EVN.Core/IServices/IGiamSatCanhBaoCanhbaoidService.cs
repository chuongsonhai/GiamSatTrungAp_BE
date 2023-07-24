using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IGiamSatCanhBaoCanhbaoidService : FX.Data.IBaseService<GiamsatCanhbaoCanhbaoid, int>
    {
        GiamsatCanhbaoCanhbaoid GetbyNo(int idloai);
        GiamsatCanhbaoCanhbaoid Getbyid(int id);
        IList<GiamsatCanhbaoCanhbaoid> GetbyCanhbao(string tungay, string denngay);
        IList<GiamsatCanhbaoCanhbaoid> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi);
    }
}
