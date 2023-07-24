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
    public class giamSatCapDienService : FX.Data.BaseService<giamSatCapDien, int>, IgiamSatCapDienService
    {
        ILog log = LogManager.GetLogger(typeof(giamSatCapDienService));
        public giamSatCapDienService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public giamSatCapDien GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        //public IList<giamSatCapDien> GetbyFilter(int TenLoaiCanhBao,  int pageindex, int pagesize, out int total)
        //{
        //    var query = Query.Where(p => p.NGUOI_GUI == TenLoaiCanhBao );
        //    total = query.Count();
        //    query = query.OrderByDescending(p => p.ID);
        //    return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        //}

        //public IList<giamSatCapDien> Filter(int maLoaiCanhBao)
        //{

        //    var query = Query.Where(p => p.ID == maLoaiCanhBao );
        //    return query.ToList();
        //}

        public bool Save(giamSatCapDien danhMucLoaiCanhBao, out string message)
        {
            message = "";
            try
            {
                IgiamSatCapDienService service = IoC.Resolve<IgiamSatCapDienService>();
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