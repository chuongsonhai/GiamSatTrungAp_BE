using EVN.Core.Domain;
using System;
using System.Collections.Generic;

namespace EVN.Core.IServices
{
    public interface ILogCanhBaoService : FX.Data.IBaseService<LogCanhBao, int>
    {
        LogCanhBao GetbyNo(int idloai);
        IList<LogCanhBao> GetbyFilter(int canhbaoID, int trangThai, string datacu, string datamoi,
            string tungay, string denngay, string nguoithuchien);        

        bool Save(LogCanhBao loaiCanhBao, out string message);


    }
}