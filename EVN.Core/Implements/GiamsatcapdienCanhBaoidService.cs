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
    public class GiamsatcapdienCanhBaoidService : FX.Data.BaseService<GiamsatcapdienCanhBaoid, int>, IGiamsatcapdienCanhBaoidService
    {
        ILog log = LogManager.GetLogger(typeof(GiamsatcapdienCanhBaoidService));
        public GiamsatcapdienCanhBaoidService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public GiamsatcapdienCanhBaoid GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }
        //public IList<GiamsatcapdienCanhBaoid> GetbyCanhbao(string  tungay)
        //{

        //    var query = Query.Where(p => p.TRANGTHAI_CANHBAO == tungay);
        //    return query.ToList();
        //}

        //public IList<GiamsatcapdienCanhBaoid> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi)
        //{

        //    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast &&  p.DONVI_DIENLUC == maDonVi &&
        //    p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
        //    return query.ToList();
        //}

    }
}