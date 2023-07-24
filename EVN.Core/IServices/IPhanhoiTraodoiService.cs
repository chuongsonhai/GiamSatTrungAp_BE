using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IPhanhoiTraodoiService : FX.Data.IBaseService<PhanhoiTraodoi, int>
    {
        PhanhoiTraodoi GetbyNo(int idloai);
        List<PhanhoiTraodoi> Getbyid(int id);
        IList<PhanhoiTraodoi> GetbyFilter(int CANHBAO_ID,  int pageindex, int pagesize, out int total);
        
        bool Save(PhanhoiTraodoi loaiCanhBao, out string message);

    }
}