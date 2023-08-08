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
    public class XacNhanTroNgaiService : FX.Data.BaseService<XacNhanTroNgai, int>, IXacNhanTroNgaiService
    {
        ILog log = LogManager.GetLogger(typeof(XacNhanTroNgaiService));
        public XacNhanTroNgaiService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public XacNhanTroNgai GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }
      
        public IList<XacNhanTroNgai> GetbyFilter(string tungay, string denngay, int trangThaiKhaoSat, string maYeuCau, string donViQuanLy
            , int pageindex, int pagesize, out int total)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast && p.TRANGTHAI == trangThaiKhaoSat
            && p.DONVI_QLY == donViQuanLy && p.DONVI_QLY == donViQuanLy);
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

 

        public XacNhanTroNgai GetKhaoSat(int id)
        {
            return Get(p => p.ID == id);
        }

        public XacNhanTroNgai UpdateKhaoid(int id)
        {
            return Get(p => p.ID == id);
        }

        public IList<XacNhanTroNgai> khaosatfilter(string tungay, string denngay, int trangThaiKhaoSat, string donViQuanLy
        , int pageindex, int pagesize, out int total)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast && p.TRANGTHAI == trangThaiKhaoSat
            && p.DONVI_QLY == donViQuanLy );
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }
        public IList<XacNhanTroNgai> GetbyCanhbao(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast);
            return query.ToList();
        }

        //public IList<XacNhanTroNgai> GetbykhachhangFilter(string tungay, string denngay, int maLoaiCanhBao, string donViQuanLy,
        //    int pageindex, int pagesize, out int total)
        //{
        //    DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //    DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //    var query = Query.Where(p => p.THOIGIAN_KHAOSAT >= tuNgayCast && p.THOIGIAN_KHAOSAT <= denNgayCast && p.CANHBAO_ID == maLoaiCanhBao
        //    && p.DONVI_QLY == donViQuanLy);
        //    total = query.Count();
        //    return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        //}
        public IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai(int ID, int TrangThaiKhaoSat)
        {
            var query = Query.Where(p => p.CANHBAO_ID == ID && p.TRANGTHAI == TrangThaiKhaoSat);
            return query.ToList();
        }

        public IList<XacNhanTroNgai> FilterByCanhBaoID(int ID)
        {
            var query = Query.Where(p => p.CANHBAO_ID == ID);
            return query.ToList();
        }
        public bool Save(XacNhanTroNgai lkhaosat, out string message)
        {
            message = "";
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
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

        public IList<XacNhanTroNgai> GetbyKhaoSat(string tungay, string denngay)
        {
            throw new NotImplementedException();
        }

        public SoLuongKhaoSatModel GetSoLuongKhaoSat(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
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