using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IBienBanKTService : FX.Data.IBaseService<BienBanKT, int>
    {
        IList<BienBanKT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);

        BienBanKT Update(BienBanKT item, BienBanDN thoathuandn, IList<ThanhPhanKT> thanhPhans);

        BienBanKT GetbyMaYCau(string maYCau);        

        bool Approve(BienBanKT item, KetQuaKT ketqua);

        bool Sign(BienBanKT item);

        bool SignRemote(BienBanKT item, byte[] pdfdata);

        bool HuyKetQua(BienBanKT bienban, KetQuaKT ketqua);

        bool Cancel(YCauNghiemThu yeucau, BienBanKT item, string noiDung);

        bool Confirm(BienBanKT item, byte[] pdfdata);

        bool KiemTraLai(BienBanKT item, out string message);
    }
}
