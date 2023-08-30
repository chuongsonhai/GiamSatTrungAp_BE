using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Implements
{
    public class XacMinhTroNgaiNewService : FX.Data.BaseService<XacMinhTroNgaiNew, int>, IXacMinhTroNgaiNewService
    {
        ILog log = LogManager.GetLogger(typeof(XacMinhTroNgaiNewService));
        public XacMinhTroNgaiNewService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo1(string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.GetAll();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            foreach (var org in listOrg)
            {
                var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == org.orgCode && p.TRANGTHAI_GQ == 0);
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
            return resultList;
        }

        public IList<BaoCaoTongHopDanhGiaMucDo> GetBaoCaoTongHopDanhGiaMucDo(string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.GetAll();
            var resultList = new List<BaoCaoTongHopDanhGiaMucDo>();
            foreach (var org in listOrg)
            {
                var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI== org.orgCode && p.TRANGTHAI_GQ == 1);
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
            return resultList;
        }

        public ChuyenKhaiThacTotal GetListChuyenKhaiThacTotal(string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);          
            var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 1);
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

        public ChuyenKhaiThacTotal GetListTroNgaiTotal(string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var chuyenKhaiThacList = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.TRANGTHAI_GQ == 0);
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

        public IList<XacMinhTroNgaiNew> GetBaoCaoChiTietMucDoHaiLong(string maDViQly, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast); 
            if(maDViQly != "-1")
            {
                query = Query.Where(p => p.NGAY >= tuNgayCast && p.NGAY <= denNgayCast && p.MA_DVI == maDViQly);
            }
            return query.ToList();
        }

   

    }
    
}
