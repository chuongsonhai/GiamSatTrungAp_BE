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
    public class LogKhaoSatService : FX.Data.BaseService<LogKhaoSat, int>, ILogKhaoSatService
    {
        ILog log = LogManager.GetLogger(typeof(LogKhaoSatService));
        public LogKhaoSatService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public LogKhaoSat GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public IList<LogKhaoSat> Filter(string tungay, string denngay, int MaKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN >= tuNgayCast && p.THOIGIAN <= denNgayCast && p.KHAOSAT_ID == MaKhaoSat);
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

        public IList<LogKhaoSat> GetByMaKhaoSat(int id)
        {
            var query = Query.Where(p => p.KHAOSAT_ID == id);
            return query.ToList();

        }

        public bool Save(LogKhaoSat danhMucLoaiKhaoSat, out string message)
        {
            message = "";
            try
            {
                ILogKhaoSatService service = IoC.Resolve<ILogKhaoSatService>();
                BeginTran();
                Save(danhMucLoaiKhaoSat);
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