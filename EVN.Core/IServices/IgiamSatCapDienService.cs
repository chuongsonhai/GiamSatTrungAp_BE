using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IgiamSatCapDienService : FX.Data.IBaseService<giamSatCapDien, int>
    {
        giamSatCapDien GetbyNo(int idloai);
        //IList<giamSatCapDien> GetbyFilter(int CANHBAO_ID,  int pageindex, int pagesize, out int total);        

        bool Save(giamSatCapDien loaiCanhBao, out string message);

    }
}