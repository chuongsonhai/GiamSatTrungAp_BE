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
    public class CanhBaoService : FX.Data.BaseService<CanhBao, int>, ICanhBaoService
    {
        ILog log = LogManager.GetLogger(typeof(CanhBaoService));
        public CanhBaoService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public CanhBao GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public CanhBao Getbyid(int id)
        {
            return Get(p => p.ID == id);
        }
        public IList<CanhBao> GetbyCanhbao(string  tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast);
            return query.ToList();
        }

        public IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy" , CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
            //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
            //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast &&  p.DONVI_DIENLUC == maDonVi &&
            p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
            return query.ToList();
        }

        public bool CreateCanhBao(CanhBao canhbao, out string message)
        {
            message = "";
            try
            {
                CreateNew(canhbao);
                CommitChanges();
                return true;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

    }
}