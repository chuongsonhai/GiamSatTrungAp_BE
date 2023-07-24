using EVN.Core.Domain;
using EVN.Core.Models;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface IKhaoSatService : FX.Data.IBaseService<KhaoSat, int>
    {
        KhaoSat GetbyNo(int idloai);
        IList<KhaoSat> GetbyFilter(int ma_canhbao, int pageindex, int pagesize, out int total);
        IList<KhaoSat> GetbyKhaoSat(string tungay, string denngay);
        bool Save(KhaoSat loaiCanhBao, out string message);
        SoLuongKhaoSatModel GetSoLuongKhaoSat(string tungay, string denngay);
    }
}