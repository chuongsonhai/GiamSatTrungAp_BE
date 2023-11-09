using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class PhanhoiTraodoiService : FX.Data.BaseService<PhanhoiTraodoi, int>, IPhanhoiTraodoiService
    {
        ILog log = LogManager.GetLogger(typeof(PhanhoiTraodoiService));
        public PhanhoiTraodoiService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public PhanhoiTraodoi GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public PhanhoiTraodoi GetbyPhanHoiId(int id)
        {
            return Get(p => p.ID == id);
        }

        public List<PhanhoiTraodoi> Getbyid(int id)
        {
            var query = Query.Where(p => p.CANHBAO_ID == id);
            return query.ToList(); 
        }

        public PhanhoiTraodoi Getbyid_phanhoi(int id)
        {
            return Get(p => p.CANHBAO_ID == id);
        }

        public IList<PhanhoiTraodoi> FilterByID(int ID)
        {
            var query = Query.Where(p => p.ID == ID);
            return query.ToList();
        }

        public IList<PhanhoiTraodoi> GetbyFilter(int ID,  int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.ID == ID);
            total = query.Count();
            query = query.OrderByDescending(p => p.ID);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public PhanhoiTraodoi Updatephanhoiid(int id)
        {
            return Get(p => p.ID == id);
        }

        public bool Save(PhanhoiTraodoi danhMucLoaiCanhBao, out string message)
        {
            message = "";
            try
            {
                ILoaiCanhBaoService service = IoC.Resolve<ILoaiCanhBaoService>();
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