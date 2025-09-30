using EVN.Core.Domain;
using FX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core.IServices
{
    public interface IHopDongService : IBaseService<HopDong, int>
    {
        bool Cancel(HopDong item);
        IList<HopDong> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);
        HopDong GetbyMaYCau(string maYCau);
        bool Notify(HopDong item, string maCViec, string deptId, string staffCode, DateTime ngayHen, string noiDung, out string message);

        bool UpdatebyCMIS(HopDong item, byte[] pdfdata);
        bool UpdatebyCMIS_SH(HopDong item, byte[] pdfdata);
    }
}
