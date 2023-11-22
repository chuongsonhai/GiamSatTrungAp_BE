using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IDvTienTrinhService : FX.Data.IBaseService<DvTienTrinh, long>
    {
        bool PushToCmis(IList<DvTienTrinh> items, out string message);

        DvTienTrinh GetbyYCau(string maYCau, string maCViec, int trangThai);

        void DongBoTienDo(YCauNghiemThu yeucau);
        void DongBoTienTrinhHU(CongVanYeuCau yeucau);

        void ThemTTrinhNT(int tthai, YCauNghiemThu yeucau, Userdata userdata);

        IList<DvTienTrinh> GetbyFilter(string maYCau, string keyword, int pageindex, int pagesize, out int total);
        IList<DvTienTrinh> GetForExport(string maYCau, string keyword);

        IList<DvTienTrinh> ListNew(string maDViQLy, string maYCau, int[] trangThais);

        long LastbyMaYCau(string maYCau);
        DvTienTrinh FilterByMaYeuCau(string ID);
        DvTienTrinh myeutop1(string ID);
        DvTienTrinh myeutopend(string ID);
    }
}
