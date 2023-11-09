using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IPhanhoiTraodoiService : FX.Data.IBaseService<PhanhoiTraodoi, int>
    {
        PhanhoiTraodoi GetbyNo(int idloai);
        PhanhoiTraodoi Getbyid_phanhoi(int id);
        PhanhoiTraodoi GetbyPhanHoiId(int id);
        List<PhanhoiTraodoi> Getbyid(int id);
        IList<PhanhoiTraodoi> GetbyFilter(int ID,  int pageindex, int pagesize, out int total);
        IList<PhanhoiTraodoi> FilterByID(int ID);
        bool Save(PhanhoiTraodoi loaiCanhBao, out string message);
        PhanhoiTraodoi Updatephanhoiid(int id);

    }
}