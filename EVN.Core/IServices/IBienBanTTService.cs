using EVN.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.IServices
{
    public interface IBienBanTTService : FX.Data.IBaseService<BienBanTT, int>
    {
        bool Cancel(BienBanTT item);
        bool UpdatebyCMIS(BienBanTT item, YCauNghiemThu yeucau, byte[] pdfdata);

        bool HuyKetQua(BienBanTT bienban, KetQuaTC ketqua);

        bool Approve(YCauNghiemThu congvan, BienBanTT item, KetQuaTC ketqua);
        BienBanTT GetbyMaYCau(string maYCau);
        IList<BienBanTT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);

        BienBanTT CreateNew(BienBanTT bienban, IList<CongTo> congTos, IList<MayBienDong> mayBienDongs, IList<MayBienDienAp> mayBienDienAps);
        BienBanTT Update(BienBanTT bienban, IList<CongTo> congTos, IList<MayBienDong> mayBienDongs, IList<MayBienDienAp> mayBienDienAps);
    }
}
