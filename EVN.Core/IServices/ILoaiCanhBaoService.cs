using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface ILoaiCanhBaoService : FX.Data.IBaseService<DanhMucLoaiCanhBao, int>
    {
        DanhMucLoaiCanhBao GetbyNo(int idloai);
        IList<DanhMucLoaiCanhBao> GetbyFilter(string TenLoaiCanhBao, int pageindex, int pagesize, out int total);        

        bool Save(DanhMucLoaiCanhBao loaiCanhBao, out string message);

        //bool Notify(BienBanDN bienban, out string message);

        //bool Cancel(BienBanDN item);

        //bool Confirm(BienBanDN item, byte[] pdfdata);

        //bool Adjust(BienBanDN item, string noiDung);

        //bool Complete(BienBanDN item, string maPBanNhan, string nVienNhan, DateTime ngayHen);
        
        //BienBanDN GetbyMaYeuCau(string maYeuCau);
    }
}