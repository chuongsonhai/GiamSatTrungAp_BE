using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface ILogCanhBaoService : FX.Data.IBaseService<LogCanhBao, int>
    {
        LogCanhBao GetbyNo(int idloai);
        //IList<LogCanhBao> GetbyFilter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string donViQuanLy);        

        bool Save(LogCanhBao loaiCanhBao, out string message);

        IList<LogCanhBao> Filter(int id);


        IList<LogCanhBao> GetByMaCanhBao(int MaCanhBao);

        LogCanhBao Getbyid_canhbaofirst(int id);

        LogCanhBao Getbyid_canhbaolast(int id);
    }
}