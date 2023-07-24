using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface ILoaiCanhBaoService : FX.Data.IBaseService<DanhMucLoaiCanhBao, int>
    {
        DanhMucLoaiCanhBao GetbyNo(int idloai);
        IList<DanhMucLoaiCanhBao> GetbyFilter(string TENLOAICANHBAO, int MALOAICANHBAO,  int pageindex, int pagesize, out int total);        

        bool Save(DanhMucLoaiCanhBao loaiCanhBao, out string message);

        IList<DanhMucLoaiCanhBao> Filter( int maLoaiCanhBao);

    }
}