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
    public class GiamSatCanhBaoCanhbaoidService : FX.Data.BaseService<GiamsatCanhbaoCanhbaoid, int>, IGiamSatCanhBaoCanhbaoidService
    {
        ILog log = LogManager.GetLogger(typeof(GiamSatCanhBaoCanhbaoidService));
        public GiamSatCanhBaoCanhbaoidService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public GiamsatCanhbaoCanhbaoid GetbyNo(int idloai)
        {
            return Get(p => p.idCanhBao == idloai);
        }

        public GiamsatCanhbaoCanhbaoid Getbyid(int id)
        {
            return Get(p => p.idCanhBao == id);
        }
        public IList<GiamsatCanhbaoCanhbaoid> GetbyCanhbao(string  tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.thoiGianGui >= tuNgayCast && p.thoiGianGui <= denNgayCast);
            return query.ToList();
        }

        public IList<GiamsatCanhbaoCanhbaoid> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
            //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
            var query = Query.Where(p => p.thoiGianGui >= tuNgayCast && p.thoiGianGui <= denNgayCast &&  p.donViQuanLy == maDonVi &&
            p.maLoaiCanhBao == maLoaiCanhBao && p.trangThai == trangThai);
            return query.ToList();
        }

    }
}