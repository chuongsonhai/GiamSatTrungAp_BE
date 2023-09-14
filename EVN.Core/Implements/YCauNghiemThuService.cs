using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.PMIS;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static iTextSharp.text.pdf.AcroFields;

namespace EVN.Core.Implements
{
    public class YCauNghiemThuService : FX.Data.BaseService<YCauNghiemThu, int>, IYCauNghiemThuService
    {
        ILog log = LogManager.GetLogger(typeof(YCauNghiemThuService));

        public YCauNghiemThuService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool CreateNew(YCauNghiemThu congvan, out string message)
        {
            message = "";
            INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
            IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
            IUserdataService userdataService = IoC.Resolve<IUserdataService>();
            ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
            IDeliverService deliver = new DeliverService();
            ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
            IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();
            NoTransaction notran = new NoTransaction(notempsrv, NoType.CVYeuCauNT);
            try
            {
                string maLoaiHSo = LoaiHSoCode.CV_NT;
                BeginTran();
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                if (string.IsNullOrWhiteSpace(congvan.SoCongVan))
                {
                    lock (notran)
                    {
                        decimal no = notran.GetNextNo();
                        congvan.SoCongVan = notran.GetCode(congvan.MaDViQLy, no);
                    }
                }
                var org = orgservice.GetbyCode(congvan.MaDViQLy);
                congvan.BenNhan = org.orgName;
                congvan.Data = congvan.GetPdf();
                congvan.Fkey = Guid.NewGuid().ToString("N");
                Save(congvan);

                hoSo.MaHoSo = congvan.Fkey;
                hoSo.MaYeuCau = congvan.MaYeuCau;
                hoSo.MaDViQLy = congvan.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "Công văn đề nghị kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";
                hoSo.Data = congvan.Data;
                hsogtosrv.Save(hoSo);
                notran.CommitTran();
                CommitTran();

                try
                {
                    var listUsser = userdataService.GetByMaCV(congvan.MaDViQLy, "TVB");
                    var cvyc = congVanYeuCauService.GetbyMaYCau(congvan.MaYeuCau);
                    if (cvyc != null)
                    {
                        var emails = listUsser.Where(p => !string.IsNullOrWhiteSpace(p.email)).Select(p => p.email).Distinct().ToArray();
                        if (emails.Count() == 0) return true;

                        var sendmailnv = new SendMail();
                        sendmailnv.EMAIL = emails[0];
                        sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                        sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                        sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                        Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                        bodyParamsNV.Add("$khachHang", cvyc.TenKhachHang ?? congvan.NguoiYeuCau);
                        bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                        bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                        bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                        bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                        bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                        bodyParamsNV.Add("$buochientai", "Chưa tiếp nhận yêu cầu kiểm tra");
                        sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                        deliver.Deliver(congvan.MaYeuCau, emails);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                RolbackTran();
                return false;
            }
        }

        public IList<YCauNghiemThu> GetbyFilter(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiNghiemThu)status);
            if (!string.IsNullOrWhiteSpace(khachhang))
                query = query.Where(p => p.CoQuanChuQuan.Contains(khachhang) || p.NguoiYeuCau.Contains(khachhang) || p.MaKHang.Contains(khachhang));
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.MaYeuCau.Contains(keyword) || p.DuAnDien.Contains(keyword));
            query = query.OrderByDescending(p => p.MaYeuCau);
            int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
            var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
            total = pageindex * pagesize + ret.Count;
            return ret.Take(pagesize).ToList();
        }

        public IList<YCauNghiemThu> GetList(string maDViQLy, DateTime fromdate, DateTime todate, out int total)
        {
            var query = Query;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);

            query = query.OrderByDescending(p => p.MaYeuCau);

            total = query.Count();
            return query.ToList();
        }

        public bool Approve(YCauNghiemThu congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message)
        {
            message = "";
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IDeliverService deliver = new DeliverService();
                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                tienTrinhSrv.DongBoTienDo(congvan);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tienTrinhSrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                BeginTran();
                congvan.TrangThai = TrangThaiNghiemThu.TiepNhan;
                congvan.MaCViec = cauhinh.MA_CVIEC_TRUOC;
                congvan.NgayDuyet = DateTime.Now;
                congvan.NguoiDuyet = userdata.maNVien;
                Update(congvan);

                DvTienTrinh tientrinh = tienTrinhSrv.GetbyYCau(congvan.MaYeuCau, maCViec, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();

                tientrinh.MA_DVIQLY = congvan.MaDViQLy;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = bPhanNhan;
                tientrinh.MA_NVIEN_NHAN = nVienNhan;

                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TRUOC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;

                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;

                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = DateTime.Today;

                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = ngayHen;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tienTrinhSrv.Save(tientrinh);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = congvan.MaYeuCau;
                tbao.MaDViQLy = congvan.MaDViQLy;
                tbao.NgayHen = ngayHen;
                tbao.MaCViec = congvan.MaCViec;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Tiếp nhận yêu cầu kiểm tra và nghiệm thu đóng điện: {congvan.MaYeuCau}, ngày tiếp nhận: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = nVienNhan;
                tbao.BPhanNhan = bPhanNhan;
                tbao.CongViec = noiDung;
                service.Save(tbao);
                CommitTran();

                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);
                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, tbao.NoiDung);

                try
                {
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == nVienNhan).FirstOrDefault();
                    var cvyc = congVanYeuCauService.GetbyMaYCau(congvan.MaYeuCau);
                    if (cvyc != null)
                    {
                        if (nhanVienNhan != null)
                        {
                            if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                            {
                                sendmailnv.EMAIL = nhanVienNhan.email;
                                sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                                sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                                sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                                Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                                bodyParamsNV.Add("$khachHang", cvyc.TenKhachHang ?? congvan.NguoiYeuCau);
                                bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                                bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                                bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                                bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                                bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                                bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                                bodyParamsNV.Add("$buochientai", "Tiếp nhận yêu cầu kiểm tra");
                                sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);

                                deliver.Deliver(congvan.MaYeuCau);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                message = ex.Message;
                RolbackTran();
                return false;
            }
        }

        public bool YeuCauKiemTra(YCauNghiemThu congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message)
        {
            message = "";
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();

                tienTrinhSrv.DongBoTienDo(congvan);

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tienTrinhSrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                IList<DvTienTrinh> tientrinhs = tienTrinhSrv.ListNew(congvan.MaDViQLy, congvan.MaYeuCau, new int[] { 0, 2 });

                BeginTran();
                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == congvan.MaCViec && p.TRANG_THAI == 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = maCViec;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tienTrinhSrv.Save(ttrinhtruoc);
                }

                congvan.TrangThai = TrangThaiNghiemThu.PhanCongKT;
                congvan.MaCViec = cauhinh.MA_CVIEC_TRUOC;
                congvan.NgayDuyet = DateTime.Now;
                congvan.NguoiDuyet = userdata.maNVien;
                Update(congvan);

                DvTienTrinh tientrinh = tienTrinhSrv.GetbyYCau(congvan.MaYeuCau, maCViec, -1);
                if (tientrinh == null || (tientrinh != null && tientrinh.TRANG_THAI == 1))
                    tientrinh = new DvTienTrinh();

                tientrinh.TRANG_THAI = 0;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = bPhanNhan;
                tientrinh.MA_NVIEN_NHAN = nVienNhan;

                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TRUOC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;

                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;

                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = DateTime.Today;

                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = ngayHen;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;

                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;
                tienTrinhSrv.Save(tientrinh);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = congvan.MaYeuCau;
                tbao.MaDViQLy = congvan.MaDViQLy;
                tbao.NgayHen = ngayHen;
                tbao.MaCViec = congvan.MaCViec;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Yêu cầu phân công kiểm tra điều kiện đóng điện điểm đấu nối, mã yêu cầu: {congvan.MaYeuCau}, ngày yêu cầu: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = nVienNhan;
                tbao.BPhanNhan = bPhanNhan;
                tbao.CongViec = noiDung;
                service.CreateNew(tbao);
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, tbao.NoiDung);
                try
                {
                    IDeliverService deliver = new DeliverService();
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == nVienNhan).FirstOrDefault();
                    var cvyc = congVanYeuCauService.GetbyMaYCau(congvan.MaYeuCau);
                    if (cvyc != null)
                    {
                        if (nhanVienNhan != null)
                        {
                            if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                            {
                                sendmailnv.EMAIL = nhanVienNhan.email;
                                sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                                sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                                sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                                Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                                bodyParamsNV.Add("$khachHang", cvyc.TenKhachHang ?? congvan.NguoiYeuCau);
                                bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                                bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                                bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                                bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                                bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                                bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                                bodyParamsNV.Add("$buochientai", "Yêu cầu kiểm tra");
                                sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                                deliver.Deliver(congvan.MaYeuCau);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                message = ex.Message;
                RolbackTran();
                return false;
            }
        }

        public YCauNghiemThu GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }

        public YCauNghiemThu SyncData(YCauNghiemThu item)
        {
            try
            {
                TreoThaoService service = new TreoThaoService();
                var result = service.LayTTinKHangTreoThao(item.MaDViQLy, item.MaYeuCau);
                if (result == null) return item;

                item.MaKHang = result.MA_DDO;
                item.MaTram = result.MA_TRAM;
                Save(item);
                CommitChanges();
                return item;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return item;
            }
        }

        /// <summary>
        /// Hủy yêu cầu kiểm tra:
        /// - Cập nhật trạng thái của công văn yêu cầu về trạng thái trước đó
        /// </summary>
        public bool CancelYeuCauKiemTra(YCauNghiemThu congvan)
        {
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                BeginTran();
                var ttrinhKTR = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, "KTR", -1);
                if (ttrinhKTR != null && ttrinhKTR.TRANG_THAI != 1)
                {
                    ttrinhKTR.TRANG_THAI = 0;
                    tientrinhsrv.Save(ttrinhKTR);
                }
                if (congvan.TrangThai > TrangThaiNghiemThu.TiepNhan)
                {
                    congvan.TrangThai = TrangThaiNghiemThu.TiepNhan;
                    congvansrv.Update(congvan);
                }
                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public void Sync()
        {
            try
            {
                ICongVanYeuCauService yeucausrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                var listHT = Query.Where(p => p.TrangThai >= TrangThaiNghiemThu.HoanThanh && p.MaCViec != "KT").ToList();
                foreach (var item in listHT)
                {
                    if (string.IsNullOrWhiteSpace(item.MaDDoDDien))
                    {
                        var ycau = yeucausrv.GetbyMaYCau(item.MaYeuCau);
                        item.MaDDoDDien = ycau.MaDDoDDien;
                    }
                    ttrinhsrv.DongBoTienDo(item);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void SyncPMIS()
        {
            try
            {
                IAF_A_ASSET_ATT_ITEMService pmisSrv = IoC.Resolve<IAF_A_ASSET_ATT_ITEMService>();
                var list = Query.Where(p => p.TrangThai == TrangThaiNghiemThu.HoanThanh && !p.DienSinhHoat);
                foreach (var item in list)
                {
                    var dongboPMIS = pmisSrv.DongBoPMIS(item);
                    if (dongboPMIS)
                    {
                        item.DienSinhHoat = true;
                        Save(item);
                    }
                }
                CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public bool Cancel(YCauNghiemThu congvan)
        {
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IHoSoGiayToService hoSoGiayToService =IoC.Resolve<IHoSoGiayToService>();
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                var item = service.GetbyMaYCau(congvan.MaYeuCau);
                BeginTran();
                var hsgt = hoSoGiayToService.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, LoaiHSoCode.CV_NT);
                if (hsgt != null)
                {
                    hoSoGiayToService.Delete(hsgt);
                }


                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var canhbao = new CanhBao();
                canhbao.LOAI_CANHBAO_ID = 12;
                canhbao.LOAI_SOLANGUI = 1;
                canhbao.MA_YC = congvan.MaYeuCau;
                canhbao.THOIGIANGUI = DateTime.Now;
                canhbao.TRANGTHAI_CANHBAO = 1;
                canhbao.DONVI_DIENLUC = congvan.MaDViQLy;
                canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.TenKhachHang + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + "ĐV: " + item.MaDViQLy + " Đã gặp trở ngại trong quá trình kiểm tra điều kiện đóng điện điểm đấu nối, đơn vị hãy xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
                ILogCanhBaoService LogCBservice = IoC.Resolve<ILogCanhBaoService>();
                string message = "";
                LogCanhBao logCB = new LogCanhBao();
                if (CBservice.CreateCanhBao(canhbao, out message))
                {
                    logCB.CANHBAO_ID = canhbao.ID;
                    logCB.DATA_MOI = JsonConvert.SerializeObject(canhbao);
                    logCB.NGUOITHUCHIEN = HttpContext.Current.User.Identity.Name;
                    logCB.THOIGIAN = DateTime.Now;
                    logCB.TRANGTHAI = 1;
                    LogCBservice.CreateNew(logCB);
                    LogCBservice.CommitChanges();
                }
                else
                {
                    throw new Exception(message);
                }


                congvansrv.Delete(congvan);
                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }
    }
}
