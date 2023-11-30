using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class LogCanhBaoService : FX.Data.BaseService<LogCanhBao, int>, ILogCanhBaoService
    {
        ILog log = LogManager.GetLogger(typeof(LogCanhBaoService));
        public LogCanhBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public LogCanhBao GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public IList<LogCanhBao> GetByMaCanhBao(int MaCanhBao)
        {

            var query = Query.Where(p => p.CANHBAO_ID == MaCanhBao);
            return query.ToList();
        }
        //public IList<LogCanhBao> GetbyFilter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string donViQuanLy)
        //{
        //    DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //    DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //    var query = Query.Where(p => p.THOIGIAN >= tuNgayCast && p.THOIGIAN <= denNgayCast && p.ID == maLoaiCanhBao &&
        //    p.TRANGTHAI == trangThai && p. );
        //    return query.ToList();
        //}

        public IList<LogCanhBao> Filter(int id)
        {
            var query = Query.Where(p => p.CANHBAO_ID == id);
            return query.ToList();


        }

        public bool Save(LogCanhBao danhMucLoaiCanhBao, out string message)
        {
            message = "";
            try
            {
                ILogCanhBaoService service = IoC.Resolve<ILogCanhBaoService>();
                BeginTran();
                Save(danhMucLoaiCanhBao);
                CommitTran();

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }
    }
}