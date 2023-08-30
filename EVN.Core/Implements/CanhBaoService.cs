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
using System.Threading.Tasks;
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
        public async Task<bool> CheckExits(string maYeuCau)
        {
            var result =  Query.Any(x => x.MA_YC == maYeuCau );
            return result;
        }

        public CanhBao Getbyid(int id)
        {
            return Get(p => p.ID == id);
        }
        public IList<CanhBao> GetbyCanhbao(string  tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast);
            return query.ToList();
        }
        public IList<CanhBao> GetbykhachhangFilter(string tungay, string denngay, int maLoaiCanhBao, string donViQuanLy, int pageindex, int pagesize, out int total)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (donViQuanLy == "-1")
            {
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && "-1" == donViQuanLy && p.TRANGTHAI_CANHBAO < 6);
                if(maLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.LOAI_CANHBAO_ID == maLoaiCanhBao);
                }
                total = query.Count();
                query = query.OrderByDescending(p => p.ID);
                return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
            }
            else
            {
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == donViQuanLy && p.TRANGTHAI_CANHBAO < 6);
                if(maLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.LOAI_CANHBAO_ID == maLoaiCanhBao);
                }
                total = query.Count();
                query = query.OrderByDescending(p => p.ID);
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
        public IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi, int solangui, string maYeuCau)
        {
            if (maDonVi == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
                //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
                //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
                
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast.AddDays(1) && "-1" == maDonVi);
                if (trangThai != -1)
                {
                    query = query.Where(p => p.TRANGTHAI_CANHBAO == trangThai);
                }
                if (solangui != 0)
                {
                    query = query.Where(p => p.LOAI_SOLANGUI == solangui);
                }
                if (maLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.LOAI_CANHBAO_ID == maLoaiCanhBao);
                }
                if (maYeuCau != "")
                {
                    query = query.Where(p => p.MA_YC == maYeuCau);
                }
                return query.ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
                //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
                //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDonVi );
                if (trangThai != -1)
                {
                    query = query.Where(p => p.TRANGTHAI_CANHBAO == trangThai);
                }
                if (solangui != 0)
                {
                    query = query.Where(p => p.LOAI_SOLANGUI == solangui);
                }
                if (maLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.LOAI_CANHBAO_ID == maLoaiCanhBao);
                }
                if (maYeuCau != "")
                {
                    query = query.Where(p => p.MA_YC == maYeuCau);
                }
                return query.ToList();
            }
        }
        public IList<CanhBao> GetAllCanhBao(out int total)
        {
            
                total = Query.Count();
                return Query.ToList();
            
        }

        public IList<CanhBao> FilterBytrangThaiAndDViQuanLy(string fromDate, string toDate, int trangThai, string DonViDienLuc)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(toDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            if (DonViDienLuc != "-1")
            {
                
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai && p.DONVI_DIENLUC  == DonViDienLuc);
                return query.ToList();
            }
            else
            {
               
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai );
                return query.ToList();
            }
            }
   

        public IList<CanhBao> FilterByMaYCauAndDViQuanLy(string fromDate, string toDate, string MaYeuCau, string DonViDienLuc)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(toDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query;
            if (DonViDienLuc != "-1")
            {
                if(MaYeuCau != "")
                {
                    query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.MA_YC == MaYeuCau && p.DONVI_DIENLUC == DonViDienLuc);
                } else
                {
                    query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == DonViDienLuc);
                }
            } else
            {
                if (MaYeuCau != "")
                {
                    query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.MA_YC == MaYeuCau);
                }
                else
                {
                    query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast);
                }
            }
            return query.ToList();
        }

        public bool CreateCanhBao(CanhBao canhbao, out string message)
        {
            message = "";
            try
            {
                CreateNew(canhbao);
              //  CommitChanges();
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