using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface ILogKhaoSatService : FX.Data.IBaseService<LogKhaoSat, int>
    {
        //LogCanhBao GetbyNo(int idloai);
        //IList<LogCanhBao> GetbyFilter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string donViQuanLy);        

        bool Save(LogKhaoSat loaiKhaoSat, out string message);

        IList<LogKhaoSat> Filter(string tungay, string denngay, int MaKhaoSat, int pageindex, int pagesize, out int total);
        IList<LogKhaoSat> GetByMaKhaoSat(int id);
    }
}