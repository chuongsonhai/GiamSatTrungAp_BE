using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface ICanhBaoService : FX.Data.IBaseService<CanhBao, int>
    {
        CanhBao GetbyNo(int idloai);
        CanhBao Getbyid(int id);
        IList<CanhBao> GetbyCanhbao(string tungay, string denngay);
        IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi);

        bool CreateCanhBao(CanhBao canhbao, out string message);
    }
}
