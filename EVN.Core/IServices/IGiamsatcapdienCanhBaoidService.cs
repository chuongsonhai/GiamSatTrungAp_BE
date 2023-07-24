using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IGiamsatcapdienCanhBaoidService : FX.Data.IBaseService<GiamsatcapdienCanhBaoid, int>
    {
        GiamsatcapdienCanhBaoid GetbyNo(int idloai);
        //IList<GiamsatcapdienCanhBaoid> GetbyCanhbao(string tungay, string denngay);
        //IList<GiamsatcapdienCanhBaoid> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi);
    }
}
