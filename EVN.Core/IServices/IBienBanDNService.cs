using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IBienBanDNService : FX.Data.IBaseService<BienBanDN, int>
    {
        BienBanDN GetbyNo(string sobienban, string mayeucau);
        IList<BienBanDN> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total);        

        bool Save(BienBanDN bienban, out string message);

        bool Notify(BienBanDN bienban, out string message);

        bool Cancel(BienBanDN item);

        bool Confirm(BienBanDN item, byte[] pdfdata);

        bool Adjust(BienBanDN item, string noiDung);

        bool Complete(BienBanDN item, string maPBanNhan, string nVienNhan, DateTime ngayHen);
        
        BienBanDN GetbyMaYeuCau(string maYeuCau);
    }
}