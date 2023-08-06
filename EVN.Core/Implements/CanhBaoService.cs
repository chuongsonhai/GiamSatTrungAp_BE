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
        public IList<CanhBao> GetbykhachhangFilter(string tungay, string denngay, int maLoaiCanhBao, string donViQuanLy,
     int pageindex, int pagesize, out int total)
        {
            if (donViQuanLy == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.LOAI_CANHBAO_ID == maLoaiCanhBao
                && "-1" == donViQuanLy);

                total = query.Count();
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.LOAI_CANHBAO_ID == maLoaiCanhBao
                && p.DONVI_DIENLUC == donViQuanLy);

                total = query.Count();
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
        }
        public SoLuongGuiModel GetSoLuongGui(string tungay, string denngay)
        {
   
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast);
            var result = new SoLuongGuiModel();

            //Số lượng
            result.soLuongDaGui = query.Count();
            result.soLuongThanhCong = query.Count(x => x.TRANGTHAI_CANHBAO == 0);
            result.soLuongThatBai = query.Count(x => x.TRANGTHAI_CANHBAO == 1);
            return result;
        }
        public IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi)
        {
            if (maDonVi == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
                //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && "-1" == maDonVi &&
                p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
                return query.ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
                //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDonVi &&
                p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
                return query.ToList();
            }
        }
        public IList<CanhBao> Filter1(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi, int pageindex, int pagesize, out int total)
        {
            if (maDonVi == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && "-1" == maDonVi &&
                p.ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
                total = query.Count();
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDonVi &&
                p.ID == maLoaiCanhBao && p.TRANGTHAI_CANHBAO == trangThai);
                total = query.Count();
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
        }

        public IList<CanhBao> FilterBytrangThaiAndDViQuanLy(string fromDate, string toDate, int trangThai, string DonViDienLuc)
        {
            if (DonViDienLuc == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(toDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai && "-1" == DonViDienLuc);
                return query.ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(toDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai && p.DONVI_DIENLUC == DonViDienLuc);
                return query.ToList();
            }
            }
   

        public IList<CanhBao> FilterByMaYCauAndDViQuanLy(string fromDate, string toDate, string MaYeuCau, string DonViDienLuc)
        {
            if (DonViDienLuc == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(toDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.MA_YC == MaYeuCau && "-1" == DonViDienLuc);
                return query.ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(fromDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(toDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.MA_YC == MaYeuCau && p.DONVI_DIENLUC == DonViDienLuc);
                return query.ToList();
            }
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