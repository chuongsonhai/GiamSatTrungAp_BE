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
    public class LoaiCanhBaoService : FX.Data.BaseService<DanhMucLoaiCanhBao, int>, ILoaiCanhBaoService
    {
        ILog log = LogManager.GetLogger(typeof(LoaiCanhBaoService));
        public LoaiCanhBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public DanhMucLoaiCanhBao GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public IList<DanhMucLoaiCanhBao> GetbyFilter(string TenLoaiCanhBao, int maLoaiCanhBao, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.TENLOAICANHBAO == TenLoaiCanhBao && p.ID == maLoaiCanhBao);
            total = query.Count();
            query = query.OrderByDescending(p => p.ID);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public IList<DanhMucLoaiCanhBao> Filter(int maLoaiCanhBao)
        {
            if(maLoaiCanhBao != 0)
            {
                var query = Query.Where(p => p.ID == maLoaiCanhBao);
                return query.ToList();
            }
            else
            {
                return Query.ToList();
            }
            
            
        }

        public bool Save(DanhMucLoaiCanhBao danhMucLoaiCanhBao, out string message)
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