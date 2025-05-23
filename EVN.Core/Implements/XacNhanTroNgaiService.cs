﻿using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EVN.Core.Implements
{
    public class XacNhanTroNgaiService : FX.Data.BaseService<XacNhanTroNgai, int>, IXacNhanTroNgaiService
    {
        ILog log = LogManager.GetLogger(typeof(XacNhanTroNgaiService));
        DataProvide_Oracle cnn = new DataProvide_Oracle();
        public XacNhanTroNgaiService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public XacNhanTroNgai GetbyNo(int idloai)
        {
            return Get(p => p.ID == idloai);
        }

        public async Task<bool> CheckExits(string maYeuCau)
        {
            var result = Query.Any(x => x.MA_YCAU == maYeuCau );
            return result;
        }
        public IList<XacNhanTroNgai> GetbyFilter(string tungay, string denngay, int trangThaiKhaoSat, string maYeuCau, string donViQuanLy
            , int pageindex, int pagesize, out int total)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI == trangThaiKhaoSat
            && p.MA_DVI == donViQuanLy && p.MA_DVI == donViQuanLy);
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

 

        public XacNhanTroNgai GetKhaoSat(int id)
        {
            return Get(p => p.ID == id);
        }

        public IList<XacNhanTroNgai> Getnotikhaosat( string madvi, string maycau)
        {
            if (madvi == "PD")
            {
                var query = Query.Where(p => "PD" == madvi && (p.DGHL_CAPDIEN == 1 || p.DGHL_CAPDIEN == 2));
                return query.ToList();
            }
            if (madvi == "X0206")
            {
                var query = Query.Where(p => (p.DGHL_CAPDIEN == 1 || p.DGHL_CAPDIEN == 2));
                return query.ToList();
            }
            else
            {
                var query = Query.Where(p => p.MA_DVI == madvi && (p.DGHL_CAPDIEN == 1 || p.DGHL_CAPDIEN == 2));
                return query.ToList();
            }
          
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
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI == trangThaiKhaoSat
            && p.MA_DVI == donViQuanLy );
            total = query.Count();
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }
        public IList<XacNhanTroNgai> GetbyCanhbao(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast);
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
        public IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai(string MA_YCAU)
        {

            var query = Query.Where(p => p.MA_YCAU == MA_YCAU).OrderBy(p=>p.NGAY);
            return query.ToList();
        }
        public IList<XacNhanTroNgai> FilterByCanhBaoIDAndTrangThai2(string MA_YCAU, int trangthai_khaosat, int mucdo_hailong)
        {
            //var query = Query.Where(p => p.TRANGTHAI_GOI == trangthai_khaosat && p.MA_YCAU == MA_YCAU && p.DGHL_CAPDIEN == mucdo_hailong).OrderBy(p => p.NGAY);
            var query = Query.Where(p => p.MA_YCAU == MA_YCAU );
            if(trangthai_khaosat != -1)
            {
                query = query.Where(p => p.TRANGTHAI_GOI == trangthai_khaosat);
            }
            if (mucdo_hailong != -1)
            {
                query = query.Where(p => p.DGHL_CAPDIEN == mucdo_hailong);
            }

            query = query.OrderBy(p => p.NGAY);
            return query.ToList();
        }


        public XacNhanTroNgai FilterByMaYeuCau(string ID)
        {
           return Get(p => p.MA_YCAU == ID);
           
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

        public SoLuongKhaoSatModel GetSoLuongKhaoSat(string madvi)
        {
            if (madvi == "-1")
            {
            var query = Query.Where(p => "-1" == madvi);
            var result = new SoLuongKhaoSatModel();
            int totalSurveys = query.Count();
            int successSurveys = query.Count(x => x.TRANGTHAI_GOI == 0);
            int failedSurveys = query.Count(x => x.TRANGTHAI_GOI == 1);
            int neutralSurveys = query.Count(x => x.TRANGTHAI_GOI == 2);

            // Tính phần trăm cho mỗi điều kiện
            double percentSuccess = (double)successSurveys / totalSurveys * 100;
            double percentFailed = (double)failedSurveys / totalSurveys * 100;
            double percentNeutral = (double)neutralSurveys / totalSurveys * 100;

            // Làm tròn phần trăm để tổng của chúng là 100
            double roundingFactor = 100.0 / (percentSuccess + percentFailed + percentNeutral);
            percentSuccess *= roundingFactor;
            percentFailed *= roundingFactor;
            percentNeutral *= roundingFactor;

            // Gán giá trị làm tròn vào model kết quả
            result.SoLuongKhaoSatThanhCong = Math.Round(percentSuccess);
            result.SoLuongKhaoSatThatBai = Math.Round(percentFailed);
            result.soLuongKhaoSatDungNgang = Math.Round(percentNeutral);
            return result;
            }
            else
            {
                var query = Query.Where(p => p.MA_DVI == madvi);
                var result = new SoLuongKhaoSatModel();
                int totalSurveys = query.Count();
                int successSurveys = query.Count(x => x.TRANGTHAI_GOI == 0);
                int failedSurveys = query.Count(x => x.TRANGTHAI_GOI == 1);
                int neutralSurveys = query.Count(x => x.TRANGTHAI_GOI == 2);

                // Tính phần trăm cho mỗi điều kiện
                double percentSuccess = (double)successSurveys / totalSurveys * 100;
                double percentFailed = (double)failedSurveys / totalSurveys * 100;
                double percentNeutral = (double)neutralSurveys / totalSurveys * 100;

                // Làm tròn phần trăm để tổng của chúng là 100
                double roundingFactor = 100.0 / (percentSuccess + percentFailed + percentNeutral);
                percentSuccess *= roundingFactor;
                percentFailed *= roundingFactor;
                percentNeutral *= roundingFactor;

                // Gán giá trị làm tròn vào model kết quả
                result.SoLuongKhaoSatThanhCong = Math.Round(percentSuccess);
                result.SoLuongKhaoSatThatBai = Math.Round(percentFailed);
                result.soLuongKhaoSatDungNgang = Math.Round(percentNeutral);
                return result;
            }
        }

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo1(string madvi, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.Getbymadvi();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            if (madvi == "-1")
            {
                foreach (var org in listOrg)
                {
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GOI != 0 && p.TRANGTHAI == 6);

                    var baoCaoTongHopDanhGiaMucDo = new BaoCaoTongHopDanhGiaMucDo();
                    baoCaoTongHopDanhGiaMucDo.DonVi = org.orgName;
                    baoCaoTongHopDanhGiaMucDo.TongSoVuCoChenhLech = query.Count(p => p.CHENH_LECH != 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienCo = query.Count(p => p.DGYC_DK_DEDANG == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienKhong = query.Count(p => p.DGYC_DK_DEDANG == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiCo = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiKhong = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepCo = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepKhong = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatCo = query.Count(p => p.DGKS_TDO_KSAT == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatKhong = query.Count(p => p.DGKS_TDO_KSAT == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienCo = query.Count(p => p.DGNT_THUAN_TIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienKhong = query.Count(p => p.DGNT_THUAN_TIEN == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachCo = query.Count(p => p.DGNT_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachKhong = query.Count(p => p.DGNT_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoCo = query.Count(p => p.DGNT_CHU_DAO == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoKhong = query.Count(p => p.DGNT_CHU_DAO == 0);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiCo = query.Count(p => p.KSAT_CHI_PHI == 1);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiKhong = query.Count(p => p.KSAT_CHI_PHI == 0);
                    baoCaoTongHopDanhGiaMucDo.RatKhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.KhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 2);
                    baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 3);
                    baoCaoTongHopDanhGiaMucDo.HaiLong = query.Count(p => p.DGHL_CAPDIEN == 4);
                    baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                    resultList.Add(baoCaoTongHopDanhGiaMucDo);
                }
            }
            else
            {
      
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GOI != 0 && p.TRANGTHAI == 6);

                    var baoCaoTongHopDanhGiaMucDo = new BaoCaoTongHopDanhGiaMucDo();
                    baoCaoTongHopDanhGiaMucDo.DonVi = madvi;
                    baoCaoTongHopDanhGiaMucDo.TongSoVuCoChenhLech = query.Count(p => p.CHENH_LECH != 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienCo = query.Count(p => p.DGYC_DK_DEDANG == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienKhong = query.Count(p => p.DGYC_DK_DEDANG == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiCo = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiKhong = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepCo = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepKhong = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatCo = query.Count(p => p.DGKS_TDO_KSAT == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatKhong = query.Count(p => p.DGKS_TDO_KSAT == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienCo = query.Count(p => p.DGNT_THUAN_TIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienKhong = query.Count(p => p.DGNT_THUAN_TIEN == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachCo = query.Count(p => p.DGNT_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachKhong = query.Count(p => p.DGNT_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoCo = query.Count(p => p.DGNT_CHU_DAO == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoKhong = query.Count(p => p.DGNT_CHU_DAO == 0);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiCo = query.Count(p => p.KSAT_CHI_PHI == 1);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiKhong = query.Count(p => p.KSAT_CHI_PHI == 0);
                    baoCaoTongHopDanhGiaMucDo.RatKhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.KhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 2);
                    baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 3);
                    baoCaoTongHopDanhGiaMucDo.HaiLong = query.Count(p => p.DGHL_CAPDIEN == 4);
                    baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                    resultList.Add(baoCaoTongHopDanhGiaMucDo);
                
            }
            return resultList;
        }

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo(string madvi, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            //var listOrg = organizationService.GetAll();
            var listOrg = organizationService.Getbymadvi();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            if (madvi == "-1")
            {
           
                    foreach (var org in listOrg)
                    {
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GOI == 0 && p.TRANGTHAI == 6);
                    var baoCaoTongHopDanhGiaMucDo = new BaoCaoTongHopDanhGiaMucDo();
                        baoCaoTongHopDanhGiaMucDo.DonVi = org.orgName;
                        baoCaoTongHopDanhGiaMucDo.TongSoVuCoChenhLech = query.Count(p => p.CHENH_LECH != 0);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienCo = query.Count(p => p.DGYC_DK_DEDANG == 1);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienKhong = query.Count(p => p.DGYC_DK_DEDANG == 0);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiCo = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiKhong = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepCo = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 1);
                        baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepKhong = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 0);
                        baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatCo = query.Count(p => p.DGKS_TDO_KSAT == 1);
                        baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatKhong = query.Count(p => p.DGKS_TDO_KSAT == 0);
                        baoCaoTongHopDanhGiaMucDo.TTDNMinhBachCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                        baoCaoTongHopDanhGiaMucDo.TTDNMinhBachKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                        baoCaoTongHopDanhGiaMucDo.TTDNChuDaoCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                        baoCaoTongHopDanhGiaMucDo.TTDNChuDaoKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienCo = query.Count(p => p.DGNT_THUAN_TIEN == 1);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienKhong = query.Count(p => p.DGNT_THUAN_TIEN == 0);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachCo = query.Count(p => p.DGNT_MINH_BACH == 1);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachKhong = query.Count(p => p.DGNT_MINH_BACH == 0);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoCo = query.Count(p => p.DGNT_CHU_DAO == 1);
                        baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoKhong = query.Count(p => p.DGNT_CHU_DAO == 0);
                        baoCaoTongHopDanhGiaMucDo.ChiPhiCo = query.Count(p => p.KSAT_CHI_PHI == 1);
                        baoCaoTongHopDanhGiaMucDo.ChiPhiKhong = query.Count(p => p.KSAT_CHI_PHI == 0);
                        baoCaoTongHopDanhGiaMucDo.RatKhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 1);
                        baoCaoTongHopDanhGiaMucDo.KhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 2);
                        baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 3);
                        baoCaoTongHopDanhGiaMucDo.HaiLong = query.Count(p => p.DGHL_CAPDIEN == 4);
                        baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                        resultList.Add(baoCaoTongHopDanhGiaMucDo);
                    }


            }
            else
            {

                var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GOI == 0 && p.TRANGTHAI == 6);


                var baoCaoTongHopDanhGiaMucDo = new BaoCaoTongHopDanhGiaMucDo();
                    baoCaoTongHopDanhGiaMucDo.DonVi = madvi;
                    baoCaoTongHopDanhGiaMucDo.TongSoVuCoChenhLech = query.Count(p => p.CHENH_LECH != 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienCo = query.Count(p => p.DGYC_DK_DEDANG == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienDeDangThuanTienKhong = query.Count(p => p.DGYC_DK_DEDANG == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiCo = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienNhanhChongKipThoiKhong = query.Count(p => p.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepCo = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 1);
                    baoCaoTongHopDanhGiaMucDo.YCauCapDienThaiDoChuyenNghiepKhong = query.Count(p => p.DGYC_THAIDO_CNGHIEP == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatCo = query.Count(p => p.DGKS_TDO_KSAT == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNTienDoKhaoSatKhong = query.Count(p => p.DGKS_TDO_KSAT == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNMinhBachKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoCo = query.Count(p => p.DGKS_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.TTDNChuDaoKhong = query.Count(p => p.DGKS_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienCo = query.Count(p => p.DGNT_THUAN_TIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuThuanTienKhong = query.Count(p => p.DGNT_THUAN_TIEN == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachCo = query.Count(p => p.DGNT_MINH_BACH == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuMinhBachKhong = query.Count(p => p.DGNT_MINH_BACH == 0);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoCo = query.Count(p => p.DGNT_CHU_DAO == 1);
                    baoCaoTongHopDanhGiaMucDo.NghiemThuChuDaoKhong = query.Count(p => p.DGNT_CHU_DAO == 0);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiCo = query.Count(p => p.KSAT_CHI_PHI == 1);
                    baoCaoTongHopDanhGiaMucDo.ChiPhiKhong = query.Count(p => p.KSAT_CHI_PHI == 0);
                    baoCaoTongHopDanhGiaMucDo.RatKhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 1);
                    baoCaoTongHopDanhGiaMucDo.KhongHaiLong = query.Count(p => p.DGHL_CAPDIEN == 2);
                    baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 3);
                    baoCaoTongHopDanhGiaMucDo.HaiLong = query.Count(p => p.DGHL_CAPDIEN == 4);
                    baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                    resultList.Add(baoCaoTongHopDanhGiaMucDo);
                
            }
            return resultList;
        }

        public ChuyenKhaiThacTotal GetListChuyenKhaiThacTotal(string madvi, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
            if (madvi == "-1")
            {
                var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GOI == 0 && p.TRANGTHAI == 6);

                var test = chuyenKhaiThacList.ToList();
                var chuyenKhaiThacTotal = new ChuyenKhaiThacTotal();
                chuyenKhaiThacTotal.TongSoVuCoChenhLech = chuyenKhaiThacList.Count(x => x.CHENH_LECH != 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienCo = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienKhong = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiCo = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiKhong = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepCo = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepKhong = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 0);
                chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatCo = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 1);
                chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatKhong = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 0);
                chuyenKhaiThacTotal.TongSoTTDNMinhBachCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoTTDNMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoTTDNChuDaoCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoTTDNChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuThuanTienCo = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuThuanTienKhong = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuMinhBachCo = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuChuDaoCo = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 0);
                chuyenKhaiThacTotal.TongSoChiPhiCo = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 1);
                chuyenKhaiThacTotal.TongSoChiPhiKhong = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 0);
                chuyenKhaiThacTotal.TongSoRatKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 1);
                chuyenKhaiThacTotal.TongSoKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 2);
                chuyenKhaiThacTotal.TongSoBinhThuong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 3);
                chuyenKhaiThacTotal.TongSoHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 4);
                chuyenKhaiThacTotal.TongSoRatHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 5);
            
            return chuyenKhaiThacTotal;
            }
            else
            {
                var chuyenKhaiThacList = Query.Where(p =>p.MA_DVI == madvi && p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GOI == 0 && p.TRANGTHAI == 6);


                var test = chuyenKhaiThacList.ToList();
                var chuyenKhaiThacTotal = new ChuyenKhaiThacTotal();
                chuyenKhaiThacTotal.TongSoVuCoChenhLech = chuyenKhaiThacList.Count(x => x.CHENH_LECH != 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienCo = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienKhong = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiCo = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiKhong = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 0);
                chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepCo = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 1);
                chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepKhong = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 0);
                chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatCo = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 1);
                chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatKhong = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 0);
                chuyenKhaiThacTotal.TongSoTTDNMinhBachCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoTTDNMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoTTDNChuDaoCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoTTDNChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuThuanTienCo = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuThuanTienKhong = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuMinhBachCo = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 0);
                chuyenKhaiThacTotal.TongSoNghiemThuChuDaoCo = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 1);
                chuyenKhaiThacTotal.TongSoNghiemThuChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 0);
                chuyenKhaiThacTotal.TongSoChiPhiCo = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 1);
                chuyenKhaiThacTotal.TongSoChiPhiKhong = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 0);
                chuyenKhaiThacTotal.TongSoRatKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 1);
                chuyenKhaiThacTotal.TongSoKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 2);
                chuyenKhaiThacTotal.TongSoBinhThuong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 3);
                chuyenKhaiThacTotal.TongSoHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 4);
                chuyenKhaiThacTotal.TongSoRatHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 5);

                return chuyenKhaiThacTotal;
            }
        }

        public ChuyenKhaiThacTotal GetListTroNgaiTotal(string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GOI != 0 && p.TRANGTHAI == 6);


            var chuyenKhaiThacTotal = new ChuyenKhaiThacTotal();
            chuyenKhaiThacTotal.TongSoVuCoChenhLech = chuyenKhaiThacList.Count(x => x.CHENH_LECH != 0);
            chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienCo = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 1);
            chuyenKhaiThacTotal.TongSoYCauCapDienDeDangThuanTienKhong = chuyenKhaiThacList.Count(x => x.DGYC_DK_DEDANG == 0);
            chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiCo = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 1);
            chuyenKhaiThacTotal.TongSoYCauCapDienNhanhChongKipThoiKhong = chuyenKhaiThacList.Count(x => x.DGYC_XACNHAN_NCHONG_KTHOI == 0);
            chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepCo = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 1);
            chuyenKhaiThacTotal.TongSoYCauCapDienThaiDoChuyenNghiepKhong = chuyenKhaiThacList.Count(x => x.DGYC_THAIDO_CNGHIEP == 0);
            chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatCo = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 1);
            chuyenKhaiThacTotal.TongSoTTDNTienDoKhaoSatKhong = chuyenKhaiThacList.Count(p => p.DGKS_TDO_KSAT == 0);
            chuyenKhaiThacTotal.TongSoTTDNMinhBachCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
            chuyenKhaiThacTotal.TongSoTTDNMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
            chuyenKhaiThacTotal.TongSoTTDNChuDaoCo = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 1);
            chuyenKhaiThacTotal.TongSoTTDNChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGKS_MINH_BACH == 0);
            chuyenKhaiThacTotal.TongSoNghiemThuThuanTienCo = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 1);
            chuyenKhaiThacTotal.TongSoNghiemThuThuanTienKhong = chuyenKhaiThacList.Count(p => p.DGNT_THUAN_TIEN == 0);
            chuyenKhaiThacTotal.TongSoNghiemThuMinhBachCo = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 1);
            chuyenKhaiThacTotal.TongSoNghiemThuMinhBachKhong = chuyenKhaiThacList.Count(p => p.DGNT_MINH_BACH == 0);
            chuyenKhaiThacTotal.TongSoNghiemThuChuDaoCo = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 1);
            chuyenKhaiThacTotal.TongSoNghiemThuChuDaoKhong = chuyenKhaiThacList.Count(p => p.DGNT_CHU_DAO == 0);
            chuyenKhaiThacTotal.TongSoChiPhiCo = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 1);
            chuyenKhaiThacTotal.TongSoChiPhiKhong = chuyenKhaiThacList.Count(p => p.KSAT_CHI_PHI == 0);
            chuyenKhaiThacTotal.TongSoRatKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 1);
            chuyenKhaiThacTotal.TongSoKhongHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 2);
            chuyenKhaiThacTotal.TongSoBinhThuong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 3);
            chuyenKhaiThacTotal.TongSoHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 4);
            chuyenKhaiThacTotal.TongSoRatHaiLong = chuyenKhaiThacList.Count(p => p.DGHL_CAPDIEN == 5);
            return chuyenKhaiThacTotal;
        }

        public IList<XacNhanTroNgai> GetBaoCaoChiTietMucDoHaiLong(string maDViQly, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
    
            var resultList = new List<XacNhanTroNgai>();
    
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            IXacNhanTroNgaiService servicetn = IoC.Resolve<IXacNhanTroNgaiService>();

            DataTable dt = new DataTable();
       
            if (maDViQly != "-1")
            {
                var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == maDViQly && p.TRANGTHAI == 6);
                var listCanhBao = query.ToList();
                foreach (var xacnhan in listCanhBao)
                {
                    dt = cnn.Get_mayc_cmis(xacnhan.MA_YCAU);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        
                    }
                    else 
                    { 
                    var TTHAI_YCAU = dt.Rows[0]["TTHAI_YCAU"].ToString();
                    int TT_HTHANH = 0;
                    if (TTHAI_YCAU == "HOÀN THÀNH")
                    {
                        TT_HTHANH = 0;
                    }
                    else
                    {
                        TT_HTHANH = 1;
                    }

                    var NGAY_TNHAN = "";
                    var NGAY_HTHANH = "";
                    if (dt.Rows.Count > 0)
                    {
                         NGAY_TNHAN = dt.Rows[0]["NGAY_TNHAN"].ToString();
                    }
                    else
                    {
                        var tientrinh = ttrinhsrv.myeutop1(xacnhan.MA_YCAU);
                        NGAY_TNHAN = tientrinh.NGAY_TAO.ToString();
                    }

                    if (dt.Rows.Count > 0)
                    {
                        NGAY_HTHANH = dt.Rows[0]["NGAY_HTHANH"].ToString();
                    }
                    else
                    {
                        var tientrinhend = ttrinhsrv.myeutopend(xacnhan.MA_YCAU);
                        NGAY_HTHANH = tientrinhend.NGAY_TAO.ToString();
                    }
                

                    var xacnhantrongai = new XacNhanTroNgai();


                    xacnhantrongai.MA_DVI = xacnhan.MA_DVI;
                    xacnhantrongai.MA_YCAU = xacnhan.MA_YCAU;
                    xacnhantrongai.MA_KH = xacnhan.MA_KH;
                    xacnhantrongai.TEN_KH = xacnhan.TEN_KH;
                    xacnhantrongai.DIA_CHI = xacnhan.DIA_CHI;
                    xacnhantrongai.DIEN_THOAI = xacnhan.DIEN_THOAI;
                    xacnhantrongai.MUCDICH_SD_DIEN = xacnhan.MUCDICH_SD_DIEN;

                    xacnhantrongai.NGAY_TIEPNHAN = DateTime.Parse(NGAY_TNHAN);
                    xacnhantrongai.NGAY_HOANTHANH = DateTime.Parse(NGAY_HTHANH) ;
                    xacnhantrongai.SO_NGAY_CT = TT_HTHANH.ToString();
                    xacnhantrongai.SO_NGAY_TH_ND = xacnhan.SO_NGAY_TH_ND;
                    xacnhantrongai.TRANGTHAI_GQ = xacnhan.TRANGTHAI_GQ;
                    xacnhantrongai.TONG_CONGSUAT_CD = xacnhan.TONG_CONGSUAT_CD;
                    xacnhantrongai.DGCD_TH_CHUONGTRINH = xacnhan.DGCD_TH_CHUONGTRINH;
                    xacnhantrongai.DGCD_TH_DANGKY = xacnhan.DGCD_TH_DANGKY;
                    xacnhantrongai.DGCD_KH_PHANHOI = xacnhan.DGCD_KH_PHANHOI;
                    xacnhantrongai.CHENH_LECH = xacnhan.CHENH_LECH;
                    xacnhantrongai.DGYC_DK_DEDANG = xacnhan.DGYC_DK_DEDANG;
                    xacnhantrongai.DGYC_XACNHAN_NCHONG_KTHOI = xacnhan.DGYC_XACNHAN_NCHONG_KTHOI;
                    xacnhantrongai.DGYC_THAIDO_CNGHIEP = xacnhan.DGYC_THAIDO_CNGHIEP;
                    xacnhantrongai.DGKS_TDO_KSAT = xacnhan.DGKS_TDO_KSAT;
                    xacnhantrongai.DGKS_MINH_BACH = xacnhan.DGKS_MINH_BACH;
                    xacnhantrongai.DGKS_CHU_DAO = xacnhan.DGKS_CHU_DAO;
                    xacnhantrongai.DGNT_THUAN_TIEN = xacnhan.DGNT_THUAN_TIEN;
                    xacnhantrongai.DGNT_MINH_BACH = xacnhan.DGNT_MINH_BACH;
                    xacnhantrongai.DGNT_CHU_DAO = xacnhan.DGNT_CHU_DAO;
                    xacnhantrongai.KSAT_CHI_PHI = xacnhan.KSAT_CHI_PHI;
                    xacnhantrongai.DGHL_CAPDIEN = xacnhan.DGHL_CAPDIEN;
                   // xacnhantrongai.TRANGTHAI_GOI = xacnhan.TRANGTHAI_GOI;
                    xacnhantrongai.NGAY = xacnhan.NGAY;
                    xacnhantrongai.NGUOI_KSAT = xacnhan.NGUOI_KSAT;
                    xacnhantrongai.Y_KIEN_KH = xacnhan.Y_KIEN_KH;
                    xacnhantrongai.PHAN_HOI = xacnhan.PHAN_HOI;
                    xacnhantrongai.NOIDUNG = xacnhan.NOIDUNG;
                    xacnhantrongai.GHI_CHU = xacnhan.GHI_CHU;
                    xacnhantrongai.TRANGTHAI = xacnhan.TRANGTHAI;
                    xacnhantrongai.TRANGTHAI_GOI = xacnhan.TRANGTHAI_GOI;

                    resultList.Add(xacnhantrongai);
                }
                }
            }
            else
            {

                var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI == 6);
                var listCanhBao2 = query.ToList();

                foreach (var xacnhan in listCanhBao2)
                {
                    dt = cnn.Get_mayc_cmis(xacnhan.MA_YCAU);
                    if (dt == null || dt.Rows.Count == 0 ) // Kiểm tra null và rỗng
                    {
                       
                    }

                    else
                    {
                        dt = cnn.Get_mayc_cmis(xacnhan.MA_YCAU);
                        var NGAY_TNHAN = "";
                        var NGAY_HTHANH = "";
                        var TTHAI_YCAU = dt.Rows[0]["TTHAI_YCAU"].ToString();
                        int TT_HTHANH = 0;
                        if (TTHAI_YCAU == "HOÀN THÀNH")
                        {
                            TT_HTHANH = 0;
                        }
                        else
                        {
                            TT_HTHANH = 1;
                        }
                        if (dt.Rows.Count > 0)
                        {
                            NGAY_TNHAN = dt.Rows[0]["NGAY_TNHAN"].ToString();
                        }
                        else
                        {
                            var tientrinh = ttrinhsrv.myeutop1(xacnhan.MA_YCAU);
                            NGAY_TNHAN = tientrinh.NGAY_TAO.ToString();
                        }

                        if (dt.Rows.Count > 0)
                        {
                            NGAY_HTHANH = dt.Rows[0]["NGAY_HTHANH"].ToString();
                        }
                        else
                        {
                            var tientrinhend = ttrinhsrv.myeutopend(xacnhan.MA_YCAU);
                            NGAY_HTHANH = tientrinhend.NGAY_TAO.ToString();
                        }

                        var xacnhantrongai = new XacNhanTroNgai();

                        xacnhantrongai.MA_DVI = xacnhan.MA_DVI;
                        xacnhantrongai.MA_YCAU = xacnhan.MA_YCAU;
                        xacnhantrongai.MA_KH = xacnhan.MA_KH;
                        xacnhantrongai.TEN_KH = xacnhan.TEN_KH;
                        xacnhantrongai.DIA_CHI = xacnhan.DIA_CHI;
                        xacnhantrongai.DIEN_THOAI = xacnhan.DIEN_THOAI;
                        xacnhantrongai.MUCDICH_SD_DIEN = xacnhan.MUCDICH_SD_DIEN;

                        xacnhantrongai.NGAY_TIEPNHAN = DateTime.Parse(NGAY_TNHAN);
                        xacnhantrongai.NGAY_HOANTHANH = DateTime.Parse(NGAY_HTHANH);

                        xacnhantrongai.SO_NGAY_CT = TT_HTHANH.ToString();
                        xacnhantrongai.SO_NGAY_TH_ND = xacnhan.SO_NGAY_TH_ND;
                        xacnhantrongai.TRANGTHAI_GQ = xacnhan.TRANGTHAI_GQ;
                        xacnhantrongai.TONG_CONGSUAT_CD = xacnhan.TONG_CONGSUAT_CD;
                        xacnhantrongai.DGCD_TH_CHUONGTRINH = xacnhan.DGCD_TH_CHUONGTRINH;
                        xacnhantrongai.DGCD_TH_DANGKY = xacnhan.DGCD_TH_DANGKY;
                        xacnhantrongai.DGCD_KH_PHANHOI = xacnhan.DGCD_KH_PHANHOI;
                        xacnhantrongai.CHENH_LECH = xacnhan.CHENH_LECH;
                        xacnhantrongai.DGYC_DK_DEDANG = xacnhan.DGYC_DK_DEDANG;
                        xacnhantrongai.DGYC_XACNHAN_NCHONG_KTHOI = xacnhan.DGYC_XACNHAN_NCHONG_KTHOI;
                        xacnhantrongai.DGYC_THAIDO_CNGHIEP = xacnhan.DGYC_THAIDO_CNGHIEP;
                        xacnhantrongai.DGKS_TDO_KSAT = xacnhan.DGKS_TDO_KSAT;
                        xacnhantrongai.DGKS_MINH_BACH = xacnhan.DGKS_MINH_BACH;
                        xacnhantrongai.DGKS_CHU_DAO = xacnhan.DGKS_CHU_DAO;
                        xacnhantrongai.DGNT_THUAN_TIEN = xacnhan.DGNT_THUAN_TIEN;
                        xacnhantrongai.DGNT_MINH_BACH = xacnhan.DGNT_MINH_BACH;
                        xacnhantrongai.DGNT_CHU_DAO = xacnhan.DGNT_CHU_DAO;
                        xacnhantrongai.KSAT_CHI_PHI = xacnhan.KSAT_CHI_PHI;
                        xacnhantrongai.DGHL_CAPDIEN = xacnhan.DGHL_CAPDIEN;
                        //  xacnhantrongai.TRANGTHAI_GOI = xacnhan.TRANGTHAI_GOI;
                        xacnhantrongai.NGAY = xacnhan.NGAY;
                        xacnhantrongai.NGUOI_KSAT = xacnhan.NGUOI_KSAT;
                        xacnhantrongai.Y_KIEN_KH = xacnhan.Y_KIEN_KH;
                        xacnhantrongai.PHAN_HOI = xacnhan.PHAN_HOI;
                        xacnhantrongai.NOIDUNG = xacnhan.NOIDUNG;

                        xacnhantrongai.GHI_CHU = xacnhan.GHI_CHU;
                        xacnhantrongai.TRANGTHAI = xacnhan.TRANGTHAI;
                        xacnhantrongai.TRANGTHAI_GOI = xacnhan.TRANGTHAI_GOI;
                        resultList.Add(xacnhantrongai);
                    }
                }
              
            }
            return resultList;
        }
    
        public IList<BaoCaoChiTietGiamSatTienDo> GetBaoCaoChiTietGiamSatTienDo(string fromdate, string todate, string maDViQly, int MaLoaiCanhBao)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
            ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();

            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast.AddDays(1));
            if (maDViQly != "-1")
            {               
                    query = query.Where(p => p.MA_DVI == maDViQly);                
            }

            IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
            var listXacNhanTroNgai = query.ToList();
            var resultList = new List<BaoCaoChiTietGiamSatTienDo>();

            foreach (var xacNhanTroNgai in listXacNhanTroNgai)
            {
                var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();
                baoCaoChiTietGiamSatTienDo.MaYeuCau = xacNhanTroNgai.MA_YCAU;
                baoCaoChiTietGiamSatTienDo.TenKhachHang = xacNhanTroNgai.TEN_KH;
                baoCaoChiTietGiamSatTienDo.DiaChi = xacNhanTroNgai.DIA_CHI;
                baoCaoChiTietGiamSatTienDo.SDT = xacNhanTroNgai.DIEN_THOAI;
                baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                baoCaoChiTietGiamSatTienDo.NgayTiepNhan = xacNhanTroNgai.NGAY_TIEPNHAN.ToString();

                var canhbao = canhBaoService.GetByMaYeuCau(baoCaoChiTietGiamSatTienDo.MaYeuCau);
                var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(baoCaoChiTietGiamSatTienDo.MaYeuCau);
                if (canhbao.LOAI_CANHBAO_ID == 1)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if(canhbao.LOAI_CANHBAO_ID == 2)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian thực hiện lập thỏa thuận đấu nối";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện đơn vị chưa thực hiện lập thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if(canhbao.LOAI_CANHBAO_ID == 3)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian tiếp nhận yêu cầu kiểm tra đóng điện và nghiệm thu";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu nghiệm thu đóng điện, đơn vị chưa thực hiện tiếp nhận yêu cầu nghiệm thu trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if (canhbao.LOAI_CANHBAO_ID == 4)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian dự thảo và ký hợp đồng mua bán điện";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu dự thảo và ký hợp đồng mua bán điện, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if (canhbao.LOAI_CANHBAO_ID == 5)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 ngày kể từ khi tiếp nhận yêu kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if(canhbao.LOAI_CANHBAO_ID == 6)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 04 ngày kể từ khi tiếp nhận yêu cầu cấp điện mới trung áp, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";

                }

                if (canhbao.LOAI_CANHBAO_ID == 7)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã gặp trở ngại trong quá trình tiếp nhận yêu cầu của khách, đơn vị hãy xử lý yêu cầu cấp điện/thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";                  
                }

                if (canhbao.LOAI_CANHBAO_ID == 8)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian thực hiện cấp điện mới trung áp";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã gặp trở ngại trong quá trình khảo sát khách hàng, đơn vị hãy xử lý yêu cầu lập thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if (canhbao.TRANGTHAI_CANHBAO == 1)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Mới tạo danh sách";
                }

                if (canhbao.TRANGTHAI_CANHBAO == 2)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Đã gửi thông báo";
                }

                if (canhbao.TRANGTHAI_CANHBAO == 3)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Đã tiếp nhận theo dõi";
                }

                if (canhbao.TRANGTHAI_CANHBAO == 4)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Chuyển đơn vị xử lý";
                }
                if (canhbao.TRANGTHAI_CANHBAO == 5)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Gửi lại cảnh báo";
                }
                if (canhbao.TRANGTHAI_CANHBAO == 6)
                {
                    baoCaoChiTietGiamSatTienDo.TrangThai = "Đã đóng cảnh báo";
                }

                baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = xacNhanTroNgai.NGAY.ToString();
                baoCaoChiTietGiamSatTienDo.NguoiGiamSat = xacNhanTroNgai.NGUOI_KSAT;
                baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = xacNhanTroNgai.Y_KIEN_KH;
                baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = "";
                baoCaoChiTietGiamSatTienDo.PhanHoi = xacNhanTroNgai.PHAN_HOI;
                baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "";
                baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = xacNhanTroNgai.NOIDUNG;
                baoCaoChiTietGiamSatTienDo.KetQua = "";
                baoCaoChiTietGiamSatTienDo.GhiChu = xacNhanTroNgai.GHI_CHU;
                resultList.Add(baoCaoChiTietGiamSatTienDo);
            }
            return resultList;
        }
    }
}