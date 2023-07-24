using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
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
    public class KhaoSatService : FX.Data.BaseService<KhaoSat, int>, IKhaoSatService
    {
        ILog log = LogManager.GetLogger(typeof(KhaoSatService));
        public KhaoSatService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public KhaoSat GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public IList<KhaoSat> GetbyFilter(int ma_canhbao, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.CANHBAO_ID == ma_canhbao);
            total = query.Count();
            query = query.OrderByDescending(p => p.ID);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public IList<KhaoSat> GetbyCanhbao(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast);
            return query.ToList();
        }

        public bool Save(KhaoSat lkhaosat, out string message)
        {
            message = "";
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                BeginTran();
                Save(lkhaosat);
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

        public IList<KhaoSat> GetbyKhaoSat(string tungay, string denngay)
        {
            throw new NotImplementedException();
        }

        public SoLuongKhaoSatModel GetSoLuongKhaoSat(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast);
            var result = new SoLuongKhaoSatModel();

            //Số lượng
            result.SoLuongKhaoSat = query.Count();
            result.SoLuongKhaoSatThanhCong = query.Count(x => x.KETQUA == "Thành công");
            result.SoLuongKhaoSatThatBai = query.Count(x => x.KETQUA == "Thất bại");
            return result;
        }

    
    
    }
}