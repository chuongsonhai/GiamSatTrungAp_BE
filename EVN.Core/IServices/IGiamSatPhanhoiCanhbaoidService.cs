using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IGiamSatPhanhoiCanhbaoidService : FX.Data.IBaseService<GiamSatPhanhoiCanhbaoid, int>
    {
        //GiamSatPhanhoiCanhbaoid GetbyNo(int idloai);
        List<GiamSatPhanhoiCanhbaoid> Getbyid(int id);
        IList<GiamSatPhanhoiCanhbaoid> GetbyFilter(int CANHBAO_ID,  int pageindex, int pagesize, out int total);
        
        bool Save(GiamSatPhanhoiCanhbaoid loaiCanhBao, out string message);

    }
}