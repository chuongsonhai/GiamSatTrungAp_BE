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
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI == trangThaiKhaoSat
            && p.MA_DVI == donViQuanLy && p.MA_DVI == donViQuanLy);
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
            var query = Query;
          //  var query = Query.Where(p => p.MA_YCAU == MA_YCAU && p.TRANGTHAI_GOI == trangthai_khaosat && p.DGHL_CAPDIEN == mucdo_hailong).OrderBy(p => p.NGAY);
            //if (!string.IsNullOrWhiteSpace(MA_YCAU))
            //    query = query.Where(p => p.MA_YCAU == MA_YCAU).OrderBy(p => p.NGAY);
            //return query.ToList();
            //if (!string.IsNullOrWhiteSpace(trangthai_khaosat.ToString()))
            //    query = query.Where(p => p.TRANGTHAI_GOI == trangthai_khaosat &&  p.MA_YCAU == MA_YCAU && p.DGHL_CAPDIEN == mucdo_hailong).OrderBy(p => p.NGAY);

            //if (!string.IsNullOrWhiteSpace(mucdo_hailong.ToString()))
                query = query.Where(p => p.TRANGTHAI_GOI == trangthai_khaosat && p.MA_YCAU == MA_YCAU && p.DGHL_CAPDIEN == mucdo_hailong).OrderBy(p => p.NGAY);

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

        public SoLuongKhaoSatModel GetSoLuongKhaoSat(string tungay, string denngay)
        {
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast);
            var result = new SoLuongKhaoSatModel();

            //Số lượng
            result.SoLuongKhaoSat = query.Count();
            result.SoLuongKhaoSatThanhCong = query.Count(x => x.TRANGTHAI == 1);
            result.SoLuongKhaoSatThatBai = query.Count(x => x.TRANGTHAI == 0);
            result.soLuongKhaoSatDungNgang = query.Count(x => x.TRANGTHAI == 2);
            return result;
        }

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo1(string madvi, string fromdate, string todate, int HangMucKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.GetAll();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            if (madvi == "-1")
            {
                foreach (var org in listOrg)
                {
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GQ == 0);
                    if (HangMucKhaoSat != -1)
                    {
                        query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GQ == 0 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                    }
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
      
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GQ == 0);
                    if (HangMucKhaoSat != -1)
                    {
                        query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GQ == 0 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                    }
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

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo(string madvi, string fromdate, string todate, int HangMucKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.GetAll();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            if (madvi == "-1")
            {
           
                    foreach (var org in listOrg)
                    {
                        var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GQ == 1);
                        if (HangMucKhaoSat != -1)
                        {
                            query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GQ == 1 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                        }
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
                        baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 4);
                        baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                        resultList.Add(baoCaoTongHopDanhGiaMucDo);
                    }


            }
            else
            {
 
                    var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GQ == 1);
                    if (HangMucKhaoSat != -1)
                    {
                        query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == madvi && p.TRANGTHAI_GQ == 1 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                    }
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
                    baoCaoTongHopDanhGiaMucDo.BinhThuong = query.Count(p => p.DGHL_CAPDIEN == 4);
                    baoCaoTongHopDanhGiaMucDo.RatHaiLong = query.Count(p => p.DGHL_CAPDIEN == 5);
                    resultList.Add(baoCaoTongHopDanhGiaMucDo);
                
            }
            return resultList;
        }

        public ChuyenKhaiThacTotal GetListChuyenKhaiThacTotal(string madvi, string fromdate, string todate, int HangMucKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            if (madvi == "-1")
            {
                var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 1);
                if (HangMucKhaoSat != -1)
                {
                    chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 1 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                }
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
                var chuyenKhaiThacList = Query.Where(p =>p.MA_DVI == madvi && p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 1);
                if (HangMucKhaoSat != -1)
                {
                    chuyenKhaiThacList = Query.Where(p => p.MA_DVI == madvi && p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 1 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                }
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

        public ChuyenKhaiThacTotal GetListTroNgaiTotal(string fromdate, string todate, int HangMucKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 0);
            if(HangMucKhaoSat != -1)
            {
                chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 0 && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
            }
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

        public IList<XacNhanTroNgai> GetBaoCaoChiTietMucDoHaiLong(string maDViQly, string fromdate, string todate, int HangMucKhaoSat)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast);
            if (maDViQly != "-1")
            {
                if (HangMucKhaoSat != -1)
                {
                    query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == maDViQly && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                }
                else
                {
                    query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == maDViQly);
                }
            }
            else
            {
                if (HangMucKhaoSat != -1)
                {
                    query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.HANGMUC_KHAOSAT == HangMucKhaoSat);
                }
            }
            return query.ToList();
        }

        public IList<BaoCaoChiTietGiamSatTienDo> GetBaoCaoChiTietGiamSatTienDo(string fromdate, string todate, string maDViQly, int MaLoaiCanhBao)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();

            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast.AddDays(1));
            if (maDViQly != "-1")
            {               
                    query = query.Where(p => p.MA_DVI == maDViQly);                
            }


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
                if(canhbao.LOAI_CANHBAO_ID == 1)
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