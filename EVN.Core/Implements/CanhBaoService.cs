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

        public CanhBao GetByMaYeuCau(string MaYeuCau)
        {
            return Get(p => p.MA_YC == MaYeuCau);
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
   
            DateTime tuNgayCast = DateTime.ParseExact(tungay, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(denngay, "d/M/yyyy", CultureInfo.InvariantCulture);
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast);
            var result = new SoLuongGuiModel();
            //Số lượng

            result.maLoaiCanhBao = query.Count(x => x.LOAI_CANHBAO_ID <= 16);
            result.soLuongDaGui = query.Count(x => x.TRANGTHAI_CANHBAO >= 2 );
            result.soLuongThanhCong = query.Count(x => x.TRANGTHAI_CANHBAO <= 6);
            result.soLuongThatBai = query.Count(x => x.TRANGTHAI_CANHBAO > 6);
            return result;
        }


        public IList<BaocaoTienDoCanhBaoModel> GetBaoCaotonghoptiendo(string maDViQly, int maloaicanhbao, string fromdate, string todate)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
         
            var data1 = new List<BaocaoTienDoCanhBaoModel>();
            IOrganizationService organizationService = IoC.Resolve<IOrganizationService>();
            var listOrg = organizationService.GetAll();
            if(maDViQly == "-1")
                {
         
                foreach (var org in listOrg)
                    {
                    var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && "-1" == maDViQly && p.LOAI_CANHBAO_ID == maloaicanhbao);
                    var data = new BaocaoTienDoCanhBaoModel();
                        data.maDvi = org.orgName;
                        data.CB_TONG = query.Count();
                        data.CB_SOCBLAN = query.Count(x => x.TRANGTHAI_CANHBAO >= 3);
                        data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                        data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                        data.NN_DNN_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                        data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                        data.NN_KH_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                        data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                        data.NN_LOI_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                        data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);

                        data1.Add(data);
                    }
                }
            else 
            {
                var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast && p.DONVI_DIENLUC == maDViQly && p.LOAI_CANHBAO_ID == maloaicanhbao);
                var data = new BaocaoTienDoCanhBaoModel();
                data.maDvi = maDViQly;
                data.CB_TONG = query.Count();
                data.CB_SOCBLAN = query.Count(x => x.TRANGTHAI_CANHBAO >= 3);
                data.CB_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9);
                data.CB_CBDVI = query.Count(x => x.LOAI_CANHBAO_ID <= 8);

                data.NN_DNN_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 1);
                data.NN_DNN_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 1);
                data.NN_DNN_TRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 1);
                data.NN_DNN_CHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 1);

                data.NN_KH_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 2);
                data.NN_KH_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 2);
                data.NN_KH_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 2);
                data.NN_KH_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 2);

                data.NN_LOI_TONG = query.Count(x => x.TRANGTHAI_CANHBAO <= 6 && x.NGUYENHHAN_CANHBAO == 3);
                data.NN_LOI_TYLE = query.Count(x => x.TRANGTHAI_CANHBAO >= 3 && x.NGUYENHHAN_CANHBAO == 3);
                data.NN_LOI_CBTRONGAI = query.Count(x => x.LOAI_CANHBAO_ID >= 9 && x.NGUYENHHAN_CANHBAO == 3);
                data.NN_LOI_CBCHAM = query.Count(x => x.LOAI_CANHBAO_ID <= 8 && x.NGUYENHHAN_CANHBAO == 3);
                data1.Add(data);
            }
            return data1;
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

       public IList<BaoCaoChiTietGiamSatTienDo> GetBaoCaoChiTietGiamSatTienDo(string maDViQly, string fromdate, string todate, int MaLoaiCanhBao)
        {
            DateTime tuNgayCast = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime denNgayCast = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);
            ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
            IXacNhanTroNgaiService xacNhanTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
            var query = Query.Where(p => p.THOIGIANGUI >= tuNgayCast && p.THOIGIANGUI <= denNgayCast.AddDays(1));
            if (maDViQly != "-1")
            {
                if (MaLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.DONVI_DIENLUC == maDViQly && p.LOAI_CANHBAO_ID == MaLoaiCanhBao);
                }
                else
                {
                    query = query.Where(p => p.DONVI_DIENLUC == maDViQly);
                }
            }
            else
            {
                if (MaLoaiCanhBao != -1)
                {
                    query = query.Where(p => p.LOAI_CANHBAO_ID == MaLoaiCanhBao);
                }
            }


            var listCanhBao = query.ToList();
            var resultList = new List<BaoCaoChiTietGiamSatTienDo>();

            foreach (var canhbao in listCanhBao)
            {
                var baoCaoChiTietGiamSatTienDo = new BaoCaoChiTietGiamSatTienDo();
                
              
                if (canhbao.LOAI_CANHBAO_ID == 1)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian tiếp nhận yêu cầu cấp điện lập thỏa thuận đấu nối của khách hàng";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện, đơn vị chưa thực hiện xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if (canhbao.LOAI_CANHBAO_ID == 2)
                {
                    baoCaoChiTietGiamSatTienDo.HangMucCanhBao = "Thời gian thực hiện lập thỏa thuận đấu nối";
                    baoCaoChiTietGiamSatTienDo.NguongCanhBao = "Đã quá 02 giờ kể từ khi tiếp nhận yêu cầu cấp điện đơn vị chưa thực hiện lập thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                }

                if (canhbao.LOAI_CANHBAO_ID == 3)
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

                if (canhbao.LOAI_CANHBAO_ID == 6)
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
                baoCaoChiTietGiamSatTienDo.MaYeuCau = canhbao.MA_YC;
                baoCaoChiTietGiamSatTienDo.MaDViQuanLy = canhbao.DONVI_DIENLUC;

                var xacNhanTroNgai = xacNhanTroNgaiService.FilterByMaYeuCau(canhbao.MA_YC);

                if (xacNhanTroNgai != null)
                {
                    baoCaoChiTietGiamSatTienDo.NgayGioGiamSat = xacNhanTroNgai.NGAY;
                    baoCaoChiTietGiamSatTienDo.NguoiGiamSat = xacNhanTroNgai.NGUOI_KSAT;
                    baoCaoChiTietGiamSatTienDo.NoiDungKhaoSat = xacNhanTroNgai.Y_KIEN_KH;
                    baoCaoChiTietGiamSatTienDo.NoiDungXuLyYKienKH = "";
                    baoCaoChiTietGiamSatTienDo.PhanHoi = xacNhanTroNgai.PHAN_HOI;
                    baoCaoChiTietGiamSatTienDo.XacMinhNguyenNhanChamGiaiQuyet = "";
                    baoCaoChiTietGiamSatTienDo.NDGhiNhanVaChuyenDonViXuLy = xacNhanTroNgai.NOIDUNG;
                    baoCaoChiTietGiamSatTienDo.KetQua = "";
                    baoCaoChiTietGiamSatTienDo.GhiChu = xacNhanTroNgai.GHI_CHU;
                    baoCaoChiTietGiamSatTienDo.TenKhachHang = xacNhanTroNgai.TEN_KH;
                    baoCaoChiTietGiamSatTienDo.DiaChi = xacNhanTroNgai.DIA_CHI;
                    baoCaoChiTietGiamSatTienDo.SDT = xacNhanTroNgai.DIEN_THOAI;
                    baoCaoChiTietGiamSatTienDo.TongCongSuatDangKy = xacNhanTroNgai.TONG_CONGSUAT_CD;
                    baoCaoChiTietGiamSatTienDo.NgayTiepNhan = xacNhanTroNgai.NGAY_TIEPNHAN;
                }
                resultList.Add(baoCaoChiTietGiamSatTienDo);
            }
            return resultList;
        }
    }

    }
