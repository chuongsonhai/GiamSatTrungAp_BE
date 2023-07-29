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
    public class GiamSatPhanhoiCanhbaoidService : FX.Data.BaseService<GiamSatPhanhoiCanhbaoid, int>, IGiamSatPhanhoiCanhbaoidService
    {
        ILog log = LogManager.GetLogger(typeof(GiamSatPhanhoiCanhbaoidService));
        public GiamSatPhanhoiCanhbaoidService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        //public GiamSatPhanhoiCanhbaoid GetbyNo(int idloai)
        //{
        //    return Get(p => p.idCanhBao == idloai);
        //}

        public List<GiamSatPhanhoiCanhbaoid> Getbyid(int id)
        {
            var query = Query.Where(p => p.idCanhBao == id && p.TRANGTHAI_XOA == 0);
            return query.ToList(); 
        }

        public IList<GiamSatPhanhoiCanhbaoid> GetbyFilter(int CANHBAO_ID,  int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.idCanhBao == CANHBAO_ID);
            total = query.Count();
            query = query.OrderByDescending(p => p.idCanhBao);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public bool Save(GiamSatPhanhoiCanhbaoid danhMucLoaiCanhBao, out string message)
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