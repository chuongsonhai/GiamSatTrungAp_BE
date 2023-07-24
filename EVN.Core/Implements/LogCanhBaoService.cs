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


        public IList<LogCanhBao> GetbyFilter(int canhbaoID, int trangThai, string datacu, string datamoi,
            string tungay, string denngay, string nguoithuchien)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.CANHBAO_ID == canhbaoID && p.TRANGTHAI == trangThai && p.DATA_CU == datacu 
            && p.DATA_MOI == datamoi && p.THOIGIAN >= tuNgayCast && p.THOIGIAN <= denNgayCast && p.NGUOITHUCHIEN == nguoithuchien);
  
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