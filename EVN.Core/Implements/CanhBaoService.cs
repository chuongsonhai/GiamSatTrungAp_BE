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
        public async Task<bool> CheckExits(string maYeuCau, int loaicanhbaoid)
        {
            var result = Query.Any(x => x.MA_YC == maYeuCau && x.LOAI_CANHBAO_ID == loaicanhbaoid ) ;
            return result;
        }

        public bool CheckExits11(string maYeuCau, int loaicanhbaoid)
        {
            var result = Query.Any(x => x.MA_YC == maYeuCau && x.LOAI_CANHBAO_ID == loaicanhbaoid);
            return result;
        }

        public CanhBao GetByMaYeuCau(string MaYeuCau)
        {
            return Get(p => p.MA_YC == MaYeuCau);
        }

        public CanhBao GetByMaYeuCautontai(string MaYeuCau, int loaicanhbaoid)
        {
            var query = Query.Where(x=> x.MA_YC == MaYeuCau && x.LOAI_CANHBAO_ID == loaicanhbaoid);
            query = query.OrderByDescending(p => p.THOIGIANGUI);
            var canhbao = query.First();
            return canhbao;
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
        public IList<SoLuongGuiModel> GetSoLuongGui(string madvi)
        {
            ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
            ILoaiCanhBaoService servicelcanhbao = IoC.Resolve<ILoaiCanhBaoService>();
            var result1 = new List<SoLuongGuiModel>();

            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.Getbymadvi();

            if (madvi == "-1")
            {

                foreach (var org in listOrg)
                {
                    var query = Query.Where(p => p.DONVI_DIENLUC == org.orgCode);
                    var result = new SoLuongGuiModel();

                    result.madvi = org.orgCode;
                    result.soLuongDaGui = query.Count(x => x.TRANGTHAI_CANHBAO >= 2);

                    result1.Add(result);
                }
            }
            else
            {

                        var query = Query.Where(p => p.DONVI_DIENLUC == madvi);
                     
                         var result = new SoLuongGuiModel();

                        result.madvi = madvi;
                        result.soLuongDaGui = query.Count(x => x.TRANGTHAI_CANHBAO >= 2);

                        result1.Add(result);
                   
            }
            

            return result1;
        }

        public IList<ThoiGianCapDienModel> Getbieudo3(string madvi)
        {
            ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
            ILoaiCanhBaoService servicelcanhbao = IoC.Resolve<ILoaiCanhBaoService>();
            var result1 = new List<ThoiGianCapDienModel>();

            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            //var query = Query.Where(p => p.LOAI_CANHBAO_ID == org.orgCode);
            //var listOrg = organizationService.Getbymadvi();
            IList<DanhMucLoaiCanhBao> listcb = servicelcanhbao.GetAll().OrderBy(p => p.ID).ToList(); // Order by ID in ascending order

            if (madvi == "-1")
            {

                foreach (var loaicb in listcb)
                {
                    var result = new ThoiGianCapDienModel();
                    var queryCB = Query.Where(x => x.LOAI_CANHBAO_ID == loaicb.ID);
                    result.loaicb = loaicb.ID;
                    result.socb = queryCB.Count();

                    result1.Add(result);
                }

            }
            else
            {
                foreach (var loaicb in listcb)
                {
                    var result = new ThoiGianCapDienModel();
                    result.loaicb = loaicb.ID;

                    // Assuming Query is a data source where you are querying data
                    var queryCB = Query.Where(x => x.LOAI_CANHBAO_ID == loaicb.ID && x.DONVI_DIENLUC == madvi);

                    // Counting the items based on the query conditions
                    result.socb = queryCB.Count();

                    // Adding the result to a list or collection
                    result1.Add(result);

                }
            }
            return result1;
        }



        public IList<BaocaoTienDoCanhBaoModel> GetBaoCaotonghoptiendo(string maDViQly, int maloaicanhbao, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);

            var data1 = new List<BaocaoTienDoCanhBaoModel>();
            ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.Getbymadvi();

            IList<CanhBao> listcb = service.GetAll();
            if (maDViQly == "-1")
                {
                if (maloaicanhbao == -1)
                {
                    foreach (var org in listOrg)
                    {
                        var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast 
                        && p.DONVI_DIENLUC == org.orgCode && -1 == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                        var data = new BaocaoTienDoCanhBaoModel();
                        data.maDvi = org.orgName;
                        data.CB_TONG = query.Count();
                        data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                        data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 );
                        data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8 );

                        data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                        if(data.NN_DNN_TONG  == 0)
                        {
                            data.NN_DNN_TYLE = 0;
                        }
                        else
                        {
                            data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                        }   
                        data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                        data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                        if (data.NN_KH_TONG == 0)
                        {
                            data.NN_KH_TYLE = 0;
                        }
                        else
                        {
                            data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                        }
                        
                        data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                        data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);
                       
                        if (data.NN_LOI_TONG == 0)
                        {
                            data.NN_LOI_TYLE = 0;
                        }
                        else
                        {
                            data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                        }
                        data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);

                        data1.Add(data);
                    }
                }
                else
                {

                    foreach (var org in listOrg)
                    {

                        var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == org.orgCode 
                        && p.LOAI_CANHBAO_ID == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                        var data = new BaocaoTienDoCanhBaoModel();
                        data.maDvi = org.orgName;
                        data.CB_TONG = query.Count();
                        data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                        data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                        data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                        data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                        if (data.NN_DNN_TONG == 0)
                        {
                            data.NN_DNN_TYLE = 0;
                        }
                        else
                        {
                            data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                        }
                        data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                        data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                        if (data.NN_KH_TONG == 0)
                        {
                            data.NN_KH_TYLE = 0;
                        }
                        else
                        {
                            data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                        }

                        data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                        data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                        if (data.NN_LOI_TONG == 0)
                        {
                            data.NN_LOI_TYLE = 0;
                        }
                        else
                        {
                            data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                        }
                        data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);

                        data1.Add(data);
                    }
                }

                }
            else 
            {
                if (maloaicanhbao == -1)
                {
                    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast 
                    && p.DONVI_DIENLUC == maDViQly && -1 == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var data = new BaocaoTienDoCanhBaoModel();
                    data.maDvi = maDViQly;
                    data.CB_TONG = query.Count();
                    data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                    data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                    data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                    data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                    if (data.NN_DNN_TONG == 0)
                    {
                        data.NN_DNN_TYLE = 0;
                    }
                    else
                    {
                        data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                    }
                    data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                    data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                    data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                    if (data.NN_KH_TONG == 0)
                    {
                        data.NN_KH_TYLE = 0;
                    }
                    else
                    {
                        data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                    }

                    data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                    data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                    data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                    if (data.NN_LOI_TONG == 0)
                    {
                        data.NN_LOI_TYLE = 0;
                    }
                    else
                    {
                        data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                    }
                    data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                    data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);
                    data1.Add(data);
                }
                else
                {
                    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast 
                    && p.DONVI_DIENLUC == maDViQly && p.LOAI_CANHBAO_ID == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var data = new BaocaoTienDoCanhBaoModel();
                    data.maDvi = maDViQly;
                    data.CB_TONG = query.Count();
                    data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                    data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                    data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                    data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                    if (data.NN_DNN_TONG == 0)
                    {
                        data.NN_DNN_TYLE = 0;
                    }
                    else
                    {
                        data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                    }
                    data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                    data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                    data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                    if (data.NN_KH_TONG == 0)
                    {
                        data.NN_KH_TYLE = 0;
                    }
                    else
                    {
                        data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                    }

                    data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                    data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                    data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                    if (data.NN_LOI_TONG == 0)
                    {
                        data.NN_LOI_TYLE = 0;
                    }
                    else
                    {
                        data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                    }
                    data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                    data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);
                    data1.Add(data);
                }
            }
            return data1;
        }

        public BaocaoTienDoCanhBaoModel GetBaoCaotonghoptiendoTong(string maDViQly, int maloaicanhbao, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);

            var data1 = new List<BaocaoTienDoCanhBaoModel>();
            ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.Getbymadvi();

            IList<CanhBao> listcb = service.GetAll();
            if (maDViQly == "-1")
            {
                if (maloaicanhbao == -1)
                {
                 
                        var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && maDViQly == "-1"  
                        && p.THOIGIANGUI <= denNgayCast  && -1 == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                        var data = new BaocaoTienDoCanhBaoModel();

                        data.CB_TONG = query.Count();
                        data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                        data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                        data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                        data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                        if (data.NN_DNN_TONG == 0)
                        {
                            data.NN_DNN_TYLE = 0;
                        }
                        else
                        {
                            data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                        }
                        data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                        data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                        if (data.NN_KH_TONG == 0)
                        {
                            data.NN_KH_TYLE = 0;
                        }
                        else
                        {
                            data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                        }

                        data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                        data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                        if (data.NN_LOI_TONG == 0)
                        {
                            data.NN_LOI_TYLE = 0;
                        }
                        else
                        {
                            data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                        }
                        data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);

                    return data;

                }
                else
                {

                  

                        var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && maDViQly == "-1"  
                        && p.THOIGIANGUI <= denNgayCast  && p.LOAI_CANHBAO_ID == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                        var data = new BaocaoTienDoCanhBaoModel();
                   
                        data.CB_TONG = query.Count();
                        data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                        data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                        data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                        data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                        if (data.NN_DNN_TONG == 0)
                        {
                            data.NN_DNN_TYLE = 0;
                        }
                        else
                        {
                            data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                        }
                        data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                        data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                        if (data.NN_KH_TONG == 0)
                        {
                            data.NN_KH_TYLE = 0;
                        }
                        else
                        {
                            data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                        }

                        data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                        data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                        if (data.NN_LOI_TONG == 0)
                        {
                            data.NN_LOI_TYLE = 0;
                        }
                        else
                        {
                            data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                        }
                        data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);

                    return data;

                }

            }

            else
            {
                if (maloaicanhbao == -1)
                {
                    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast 
                    && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDViQly && -1 == maloaicanhbao 
                    && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var data = new BaocaoTienDoCanhBaoModel();
                    data.maDvi = maDViQly;
                    data.CB_TONG = query.Count();
                    data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                    data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                    data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                    data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                    if (data.NN_DNN_TONG == 0)
                    {
                        data.NN_DNN_TYLE = 0;
                    }
                    else
                    {
                        data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                    }
                    data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                    data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                    data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                    if (data.NN_KH_TONG == 0)
                    {
                        data.NN_KH_TYLE = 0;
                    }
                    else
                    {
                        data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                    }

                    data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                    data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                    data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                    if (data.NN_LOI_TONG == 0)
                    {
                        data.NN_LOI_TYLE = 0;
                    }
                    else
                    {
                        data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                    }
                    data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                    data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);
                   
                    return data;
                }
                else
                {
                    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDViQly 
                    && p.LOAI_CANHBAO_ID == maloaicanhbao && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var data = new BaocaoTienDoCanhBaoModel();

                    data.CB_TONG = query.Count();
                    data.CB_SOCBLAN = query.Count(x => x.LOAI_SOLANGUI > 1);
                    data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                    data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                    data.NN_DNN_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 1);
                    if (data.NN_DNN_TONG == 0)
                    {
                        data.NN_DNN_TYLE = 0;
                    }
                    else
                    {
                        data.NN_DNN_TYLE = (data.NN_DNN_TONG) / ((data.CB_TONG));
                    }
                    data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                    data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                    data.NN_KH_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 2);
                    if (data.NN_KH_TONG == 0)
                    {
                        data.NN_KH_TYLE = 0;
                    }
                    else
                    {
                        data.NN_KH_TYLE = (data.NN_KH_TONG) / ((data.CB_TONG));
                    }

                    data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                    data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                    data.NN_LOI_TONG = query.Count(x => x.LOAI_SOLANGUI >= 1 && x.NGUYENHHAN_CANHBAO == 3);

                    if (data.NN_LOI_TONG == 0)
                    {
                        data.NN_LOI_TYLE = 0;
                    }
                    else
                    {
                        data.NN_LOI_TYLE = (data.NN_LOI_TONG) / ((data.CB_TONG));
                    }
                    data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                    data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);
                    return data;
                }
            }
            
        }

        public IList<CanhBao> Filter(string tungay, string denngay, int maLoaiCanhBao, int trangThai, string maDonVi, int solangui, string maYeuCau, int pageindex, int pagesize, out int total)
        {
            if (maDonVi == "-1")
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
                //var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO == trangThai
                //&& p.LOAI_CANHBAO_ID == maLoaiCanhBao && p.DONVI_DIENLUC == maDonVi);
                
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && "-1" == maDonVi);
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
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + ret.Count;
                return ret.Take(pagesize).ToList();
            }
            else
            {
                DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
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
                int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
                var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
                total = pageindex * pagesize + ret.Count;
                return ret.Take(pagesize).ToList();
            }
        }
        public IList<CanhBao> GetAllCanhBao(out int total)
        {
            DateTime a = DateTime.Now.AddDays(-30);
            var query = Query;
            query = query.Where(p => p.THOIGIANGUI >= a);
            total = query.Count();
            return query.ToList();

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

       public IList<BaoCaoChiTietGiamSatTienDo> GetBaoCaoChiTietGiamSatTienDo(string maDViQly, string fromdate, string todate, int MaLoaiCanhBao)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture).AddDays(1);
            ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
            IXacNhanTroNgaiService xacNhanTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
            IGiamSatCongVanCanhbaoidService serviceyeucau = IoC.Resolve<IGiamSatCongVanCanhbaoidService>();
            IYCauNghiemThuService NTservice = IoC.Resolve<IYCauNghiemThuService>();
           // var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
          
            var resultList = new List<BaoCaoChiTietGiamSatTienDo>();
                if (maDViQly == "-1")
                {
                    if (MaLoaiCanhBao == -1)
                    {

                      var  query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && maDViQly == "-1" &&  -1 == MaLoaiCanhBao 
                      && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0 );
                        var listCanhBao = query.ToList();
                    foreach (var canhbao in listCanhBao)
                    {
                        var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();


                        if (canhbao.LOAI_CANHBAO_ID == 1)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 2)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 3)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 4)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian dự thảo và ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 5)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 6)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;

                        }

                        if (canhbao.LOAI_CANHBAO_ID == 7)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu cấp điện/thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 8)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trờ ngại khảo sát lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 9)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 10)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 11)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 12)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi kiểm tra điều kiện đóng điện điểm đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 13)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi thi công treo tháo";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 14)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 15)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 16)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
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
                        baoCaoChiTietGiamSatTienDo.MaYeuCau = canhbao.MA_YC;
                        baoCaoChiTietGiamSatTienDo.MaDViQuanLy = canhbao.DONVI_DIENLUC;

                        var xacNhanTroNgai = xacNhanTroNgaiService.FilterByMaYeuCau(canhbao.MA_YC);
                        var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(canhbao.MA_YC);
                        var YCNT = NTservice.GetbyMaYCau(canhbao.MA_YC);

                        IPhanhoiTraodoiService phanhoiService = IoC.Resolve<IPhanhoiTraodoiService>();

                        var id_canhbao = canhBaoService.Getbyid(canhbao.ID);
                        var id_phanhoi = phanhoiService.Getbyid_phanhoi(canhbao.ID);

                        baoCaoChiTietGiamSatTienDo.id = canhbao.ID;
                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = id_phanhoi.THOIGIAN_GUI.ToString();
                        }

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = id_phanhoi.NGUOI_PHANHOI_X3;
                        }



                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = id_phanhoi.NOIDUNG_PHANHOI_X3;
                        }

                        //if (xacNhanTroNgai == null)
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = xacNhanTroNgai.Y_KIEN_KH;

                        //}

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.PhanHoi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.PhanHoi = id_phanhoi.NOIDUNG_PHANHOI;
                        }



                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "1")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Ngành điện";
                        }

                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "2")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Khách quan";
                        }
                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "3")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Chương trình";
                        }

                        //if (id_phanhoi == null)
                        //{

                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = id_phanhoi.NOIDUNG_PHANHOI;
                        //}

                        baoCaoChiTietGiamSatTienDo.KetQua = id_canhbao.KETQUA_GIAMSAT;
                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = ThongTinYeuCau.TenKhachHang;
                        }

                        if (YCNT == null)
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = YCNT.DiaChi;

                        }

                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = ThongTinYeuCau.DienThoai;

                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = xacNhanTroNgai.NGAY_TIEPNHAN.ToString();
                        }


                        resultList.Add(baoCaoChiTietGiamSatTienDo);
                    }

                }
                    else
                    {
                       var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && maDViQly == "-1" &&  p.LOAI_CANHBAO_ID == MaLoaiCanhBao 
                        && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var listCanhBao1 = query.ToList();
                    foreach (var canhbao in listCanhBao1)
                    {
                        var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();


                        if (canhbao.LOAI_CANHBAO_ID == 1)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 2)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 3)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 4)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian dự thảo và ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 5)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 6)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;

                        }

                        if (canhbao.LOAI_CANHBAO_ID == 7)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu cấp điện/thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 8)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trờ ngại khảo sát lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 9)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 10)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 11)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 12)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi kiểm tra điều kiện đóng điện điểm đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 13)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi thi công treo tháo";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 14)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 15)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 16)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
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
                        baoCaoChiTietGiamSatTienDo.MaYeuCau = canhbao.MA_YC;
                        baoCaoChiTietGiamSatTienDo.MaDViQuanLy = canhbao.DONVI_DIENLUC;

                        var xacNhanTroNgai = xacNhanTroNgaiService.FilterByMaYeuCau(canhbao.MA_YC);
                        var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(canhbao.MA_YC);
                        var YCNT = NTservice.GetbyMaYCau(canhbao.MA_YC);

                        IPhanhoiTraodoiService phanhoiService = IoC.Resolve<IPhanhoiTraodoiService>();

                        var id_canhbao = canhBaoService.Getbyid(canhbao.ID);
                        var id_phanhoi = phanhoiService.Getbyid_phanhoi(canhbao.ID);
                        baoCaoChiTietGiamSatTienDo.id = canhbao.ID;

                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = id_phanhoi.THOIGIAN_GUI.ToString();
                        }

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = id_phanhoi.NGUOI_PHANHOI_X3;
                        }



                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = id_phanhoi.NOIDUNG_PHANHOI_X3;
                        }

                        //if (xacNhanTroNgai == null)
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = xacNhanTroNgai.Y_KIEN_KH;

                        //}

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.PhanHoi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.PhanHoi = id_phanhoi.NOIDUNG_PHANHOI;
                        }



                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "1")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Ngành điện";
                        }

                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "2")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Khách quan";
                        }
                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "3")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Chương trình";
                        }

                        //if (id_phanhoi == null)
                        //{

                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = id_phanhoi.NOIDUNG_PHANHOI;
                        //}

                        baoCaoChiTietGiamSatTienDo.KetQua = id_canhbao.KETQUA_GIAMSAT;
                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = ThongTinYeuCau.TenKhachHang;
                        }

                        if (YCNT == null)
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = YCNT.DiaChi;

                        }

                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = ThongTinYeuCau.DienThoai;

                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = xacNhanTroNgai.NGAY_TIEPNHAN.ToString();
                        }


                        resultList.Add(baoCaoChiTietGiamSatTienDo);
                    }
                }
                }


                else
                {

                if (MaLoaiCanhBao == -1)
                {
               var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast &&  p.DONVI_DIENLUC == maDViQly && -1 == MaLoaiCanhBao
                && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var listCanhBao2 = query.ToList();
                    foreach (var canhbao in listCanhBao2)
                    {
                        var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();


                        if (canhbao.LOAI_CANHBAO_ID == 1)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 2)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 3)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 4)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian dự thảo và ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 5)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 6)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;

                        }

                        if (canhbao.LOAI_CANHBAO_ID == 7)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu cấp điện/thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 8)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trờ ngại khảo sát lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 9)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 10)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 11)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 12)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi kiểm tra điều kiện đóng điện điểm đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 13)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi thi công treo tháo";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 14)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 15)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 16)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
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
                        baoCaoChiTietGiamSatTienDo.MaYeuCau = canhbao.MA_YC;
                        baoCaoChiTietGiamSatTienDo.MaDViQuanLy = canhbao.DONVI_DIENLUC;

                        var xacNhanTroNgai = xacNhanTroNgaiService.FilterByMaYeuCau(canhbao.MA_YC);
                        var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(canhbao.MA_YC);
                        var YCNT = NTservice.GetbyMaYCau(canhbao.MA_YC);

                        IPhanhoiTraodoiService phanhoiService = IoC.Resolve<IPhanhoiTraodoiService>();

                        var id_canhbao = canhBaoService.Getbyid(canhbao.ID);
                        var id_phanhoi = phanhoiService.Getbyid_phanhoi(canhbao.ID);

                        baoCaoChiTietGiamSatTienDo.id = canhbao.ID;
                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = id_phanhoi.THOIGIAN_GUI.ToString();
                        }

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = id_phanhoi.NGUOI_PHANHOI_X3;
                        }



                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = id_phanhoi.NOIDUNG_PHANHOI_X3;
                        }

                        //if (xacNhanTroNgai == null)
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = xacNhanTroNgai.Y_KIEN_KH;

                        //}

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.PhanHoi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.PhanHoi = id_phanhoi.NOIDUNG_PHANHOI;
                        }



                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "1")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Ngành điện";
                        }

                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "2")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Khách quan";
                        }
                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "3")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Chương trình";
                        }

                        //if (id_phanhoi == null)
                        //{

                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = id_phanhoi.NOIDUNG_PHANHOI;
                        //}

                        baoCaoChiTietGiamSatTienDo.KetQua = id_canhbao.KETQUA_GIAMSAT;
                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = ThongTinYeuCau.TenKhachHang;
                        }

                        if (YCNT == null)
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = YCNT.DiaChi;

                        }

                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = ThongTinYeuCau.DienThoai;

                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = xacNhanTroNgai.NGAY_TIEPNHAN.ToString();
                        }

                        resultList.Add(baoCaoChiTietGiamSatTienDo);
                    }
                }
                else
                {
                   var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.LOAI_CANHBAO_ID == MaLoaiCanhBao 
                    && p.DONVI_DIENLUC == maDViQly && p.TRANGTHAI_CANHBAO >= 2 && p.TRANGTHAI_CANHBAO <= 6 && p.NGUYENHHAN_CANHBAO != 0);
                    var listCanhBao3 = query.ToList();
                    foreach (var canhbao in listCanhBao3)
                    {
                        var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();


                        if (canhbao.LOAI_CANHBAO_ID == 1)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 2)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 3)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian tiếp nhận yêu cầu kiểm tra điểm đóng điện và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 4)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian dự thảo và ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 5)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện kiểm tra điều kiện kỹ thuật điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 6)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian thực hiện cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;

                        }

                        if (canhbao.LOAI_CANHBAO_ID == 7)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu cấp điện/thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 8)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trờ ngại khảo sát lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 9)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu lập thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 10)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát việc từ chối tiếp nhận yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 11)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 12)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi kiểm tra điều kiện đóng điện điểm đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 13)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát trở ngại khi thi công treo tháo";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 14)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát nguyên nhân khách hàng từ chối ký hợp đồng mua bán điện";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 15)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Giám sát thời gian nghiệm thu yêu cầu cấp điện mới trung áp";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
                        }

                        if (canhbao.LOAI_CANHBAO_ID == 16)
                        {
                            baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Cảnh báo các bộ hồ sơ sắp hết hạn hiệu lực thỏa thuận đấu nối";
                            baoCaoChiTietGiamSatTienDo.NguongCanhBao = canhbao.NOIDUNG;
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
                        baoCaoChiTietGiamSatTienDo.MaYeuCau = canhbao.MA_YC;
                        baoCaoChiTietGiamSatTienDo.MaDViQuanLy = canhbao.DONVI_DIENLUC;

                        var xacNhanTroNgai = xacNhanTroNgaiService.FilterByMaYeuCau(canhbao.MA_YC);
                        var ThongTinYeuCau = serviceyeucau.GetbyMaYCau(canhbao.MA_YC);
                        var YCNT = NTservice.GetbyMaYCau(canhbao.MA_YC);

                        IPhanhoiTraodoiService phanhoiService = IoC.Resolve<IPhanhoiTraodoiService>();

                        var id_canhbao = canhBaoService.Getbyid(canhbao.ID);
                        var id_phanhoi = phanhoiService.Getbyid_phanhoi(canhbao.ID);

                        baoCaoChiTietGiamSatTienDo.id = canhbao.ID;
                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = xacNhanTroNgai.NGAY_TIEPNHAN.ToString();
                        }

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.NguoiGiamSat = id_phanhoi.NGUOI_PHANHOI_X3;
                        }



                        if (id_phanhoi == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = id_phanhoi.NOIDUNG_PHANHOI_X3;
                        }

                        //if (xacNhanTroNgai == null)
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = xacNhanTroNgai.Y_KIEN_KH;

                        //}

                        if (id_phanhoi == null)
                        {

                            baoCaoChiTietGiamSatTienDo.PhanHoi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.PhanHoi = id_phanhoi.NOIDUNG_PHANHOI;
                        }



                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "1")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Ngành điện";
                        }

                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "2")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Khách quan";
                        }
                        if (id_canhbao.NGUYENHHAN_CANHBAO.ToString() == "3")
                        {
                            baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "Do Chương trình";
                        }

                        //if (id_phanhoi == null)
                        //{

                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = null;
                        //}
                        //else
                        //{
                        //    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = id_phanhoi.NOIDUNG_PHANHOI;
                        //}

                        baoCaoChiTietGiamSatTienDo.KetQua = id_canhbao.KETQUA_GIAMSAT;
                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.TenKhachHang = ThongTinYeuCau.TenKhachHang;
                        }

                        if (YCNT == null)
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.DiaChi = YCNT.DiaChi;

                        }

                        if (ThongTinYeuCau == null)
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = null;
                        }
                        else
                        {
                            baoCaoChiTietGiamSatTienDo.SDT = ThongTinYeuCau.DienThoai;

                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                        }

                        if (xacNhanTroNgai == null)
                        {
                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = null;
                        }
                        else
                        {

                            baoCaoChiTietGiamSatTienDo.NgayTiepNhan = id_phanhoi.THOIGIAN_GUI.ToString(); 
                        }


                        resultList.Add(baoCaoChiTietGiamSatTienDo);
                    }

                }
            
            }
            return resultList;
        }
    }

    }
