using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class CongVanYeuCauService : FX.Data.BaseService<CongVanYeuCau, int>, ICongVanYeuCauService
    {
        private ILog log = LogManager.GetLogger(typeof(CongVanYeuCauService));
        public CongVanYeuCauService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }


        public IList<CongVanYeuCau> GetbyFilter(string maDViQLy, string keyword, string khachhang, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);
            if (status > -1)
                query = query.Where(p => p.TrangThai == (TrangThaiCongVan)status);
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

        public IList<CongVanYeuCau> GetThongKe(string maDViQLy, string keyword, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            ;
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.CoQuanChuQuan.Contains(keyword) || p.NguoiYeuCau.Contains(keyword) || p.MaKHang.Contains(keyword) || p.MaYeuCau.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);
            int maxtemp = pageindex <= 1 ? 4 - pageindex : 2;//load tối đa 2 trang tiếp theo, nếu page =1 hoặc 2 thì sẽ load 4 trang hoặc 3 trang
            var ret = query.Skip(pageindex * pagesize).Take(pagesize * maxtemp + 1).ToList();
            total = pageindex * pagesize + ret.Count;
            return ret.Take(pagesize).ToList();
        }

        public IList<CongVanYeuCau> GetThongKeExport(string maDViQLy, string keyword, DateTime fromdate, DateTime todate)
        {
            var query = Query;
            if (fromdate != DateTime.MinValue)
                query = query.Where(p => p.NgayYeuCau >= fromdate && p.NgayYeuCau < todate.AddDays(1));
            if (todate != DateTime.MaxValue)
                query = query.Where(p => p.NgayYeuCau < todate.AddDays(1));
            ;
            if (!string.IsNullOrWhiteSpace(maDViQLy))
                query = query.Where(p => p.MaDViQLy == maDViQLy);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.CoQuanChuQuan.Contains(keyword) || p.NguoiYeuCau.Contains(keyword) || p.MaKHang.Contains(keyword) || p.MaYeuCau.Contains(keyword));

            query = query.OrderByDescending(p => p.MaYeuCau);

            return query.ToList();
        }
        public IList<CongVanYeuCau> GetList(string maDViQLy, DateTime fromdate, DateTime todate, out int total)
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
        public bool CreateNew(CongVanYeuCau congvan, out string message)
        {
            message = "";
            INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
            IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
            NoTransaction notran = new NoTransaction(notempsrv, NoType.CVYeuCauDN);
            try
            {
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, LoaiHSoCode.CV_DN);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                if (string.IsNullOrWhiteSpace(congvan.SoCongVan))
                {
                    lock (notran)
                    {
                        decimal no = notran.GetNextNo();
                        congvan.SoCongVan = notran.GetCode(congvan.MaDViQLy, no);
                    }
                }
                congvan.Data = congvan.GetPdf();
                congvan.Fkey = Guid.NewGuid().ToString("N");

                BeginTran();
                CreateNew(congvan);

                hoSo.MaHoSo = congvan.Fkey;
                hoSo.TrangThai = 1;
                hoSo.MaYeuCau = congvan.MaYeuCau;
                hoSo.MaDViQLy = congvan.MaDViQLy;
                hoSo.LoaiHoSo = LoaiHSoCode.CV_DN;
                hoSo.TenHoSo = "Công văn đề nghị đấu nối vào lưới điện trung áp";
                hoSo.Data = congvan.Data;
                hsogtosrv.Save(hoSo);

                notran.CommitTran();
                CommitTran();

                TimeSpan hourMinute;
                hourMinute = congvan.NgayYeuCau.TimeOfDay;
                IDeliverService deliver = new DeliverService();
                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IUserdataService userdataservice = IoC.Resolve<IUserdataService>();
                if (hourMinute >= new TimeSpan(16, 0, 0))
                {
                    try
                    {
                        var sendmail = new SendMail();
                        sendmail.EMAIL = congvan.Email;
                        sendmail.TIEUDE = "Thông báo về thời gian xử lý yêu cầu";
                        sendmail.MA_DVIQLY = congvan.MaDViQLy;
                        sendmail.MA_YCAU_KNAI = congvan.MaYeuCau;
                        Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                        bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                        bodyParams.Add("$donVi", congvan.MaDViQLy);
                        bodyParams.Add("$duAnDien", congvan.DuAnDien);
                        bodyParams.Add("$khuVuc", congvan.DiaChiDungDien);
                        bodyParams.Add("$maYCau", congvan.MaYeuCau);
                        bodyParams.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));                        
                        sendmailsrv.Process(sendmail, "CanhBaoSau14H", bodyParams);
                        deliver.Deliver(congvan.MaYeuCau);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
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

        public bool DuyetHoSo(CongVanYeuCau congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message)
        {
            message = "";
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                IMaDichVuService dvservice = IoC.Resolve<IMaDichVuService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();

                long nextstep = tienTrinhSrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                IList<DvTienTrinh> tientrinhs = new List<DvTienTrinh>();
                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = bPhanNhan;
                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TRUOC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_NVIEN_NHAN = nVienNhan;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = congvan.NgayYeuCau;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = ngayHen;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;

                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                TimeSpan variable = tientrinh.NGAY_KTHUC.Value.Date - tientrinh.NGAY_BDAU.Date;
                var songay = Math.Round(variable.TotalDays, 1, MidpointRounding.AwayFromZero) + 1;
                tientrinh.SO_NGAY_LVIEC = songay.ToString();
                if (string.IsNullOrWhiteSpace(congvan.TenKhachHang))
                    congvan.TenKhachHang = congvan.NguoiYeuCau;

                congvan.NgayDuyet = DateTime.Now;

                congvan.NguoiLap = userdata.maNVien;
                congvan.NguoiDuyet = userdata.maNVien;
                
                congvan.TrangThai = TrangThaiCongVan.TiepNhan;
                congvan.MaCViec = cauhinh.MA_CVIEC_TRUOC;

                ICmisProcessService cmisProcess = new CmisProcessService();
                log.ErrorFormat("Tiep nhan CMIS tienTrinh:{0}", JsonConvert.SerializeObject(tientrinh).ToString());
                log.ErrorFormat("Tiep nhan CMIS congvan:{0}", JsonConvert.SerializeObject(congvan).ToString());
                var tiepnhan = cmisProcess.TiepNhanYeuCau(congvan, tientrinh);
                log.Error($"Tiep nhan CMIS: {tiepnhan}");
                if (!tiepnhan)
                    return false;

                string maLoaiHSo = LoaiHSoCode.CV_DN;
                IRepository repository = new FileStoreRepository();
                var pdfdata = repository.GetData(congvan.Data);
                if (pdfdata != null)
                {
                    if (!cmisProcess.UploadPdf(congvan.MaDViQLy, congvan.MaYeuCau, pdfdata, maLoaiHSo))
                    {
                        log.Error($"Lỗi upload file lên CMIS: {congvan.MaYeuCau} - {maLoaiHSo}");                        
                    }
                }

                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);

                BeginTran();
                Save(congvan);
                if (hoSo == null)
                    hoSo = new HoSoGiayTo();
                hoSo.MaHoSo = congvan.Fkey;
                hoSo.TrangThai = 1;
                hoSo.MaYeuCau = congvan.MaYeuCau;
                hoSo.MaDViQLy = congvan.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.TenHoSo = "Công văn đề nghị đấu nối vào lưới điện trung áp";
                hoSo.Data = congvan.Data;
                hsogtosrv.Save(hoSo);

                tientrinh.TRANG_THAI = 1;
                tienTrinhSrv.CreateNew(tientrinh);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = congvan.MaYeuCau;
                tbao.MaDViQLy = congvan.MaDViQLy;
                tbao.NgayHen = ngayHen;
                tbao.MaCViec = congvan.MaCViec;
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.DaXuLy;
                tbao.NoiDung = $"Tiếp nhận yêu cầu: {congvan.MaYeuCau}, ngày tiếp nhận: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = nVienNhan;
                tbao.BPhanNhan = bPhanNhan;
                tbao.CongViec = noiDung;
                service.CreateNew(tbao);

                var madvu = dvservice.Getbykey(congvan.MaYeuCau);
                if (madvu == null)
                {
                    madvu = new MaDichVu();
                    madvu.MA_YCAU_KNAI = congvan.MaYeuCau;
                    madvu.MA_DVIQLY = congvan.MaDViQLy;
                    madvu.NOI_DUNG_YCAU = congvan.NoiDungYeuCau;
                    madvu.TEN_KHANG = congvan.TenKhachHang;
                    madvu.EMAIL = congvan.Email;
                    madvu.DTHOAI = congvan.DienThoai;
                    madvu.ID_WEB = CommonUtils.RandomValue(8);
                    dvservice.CreateNew(madvu);
                }
                CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();
                try
                {
                    var sendmail = new SendMail();
                    sendmail.EMAIL = congvan.Email;
                    sendmail.TIEUDE = "Tiếp nhận yêu cầu đề nghị đấu nối vào lưới điện trung áp";
                    sendmail.MA_DVIQLY = congvan.MaDViQLy;
                    sendmail.MA_YCAU_KNAI = congvan.MaYeuCau;
                    Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                    bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                    bodyParams.Add("$maYCau", congvan.MaYeuCau);
                    bodyParams.Add("$maDichVu", madvu.ID_WEB);
                    bodyParams.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                    bodyParams.Add("$ngayDuyet", congvan.NgayDuyet.ToString("dd/MM/yyyy"));
                    sendmailsrv.Process(sendmail, "YeuCauDauNoi", bodyParams);
                    deliver.Deliver(congvan.MaYeuCau);

                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == nVienNhan).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Tiếp nhận yêu cầu");
                            sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                            deliver.Deliver(congvan.MaYeuCau);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, tbao.NoiDung);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                RolbackTran();
                return false;
            }
        }

        public bool YeuCauKhaoSat(CongVanYeuCau congvan, string maCViec, string bPhanNhan, string nVienNhan, DateTime ngayHen, string noiDung, out string message)
        {
            message = "";
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();

                long nextstep = tienTrinhSrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                BeginTran();
                congvan.TrangThai = TrangThaiCongVan.PhanCongKS;
                congvan.MaCViec = cauhinh.MA_CVIEC_TRUOC;
                congvan.NgayDuyet = DateTime.Now;
                congvan.NguoiDuyet = userdata.maNVien;
                Save(congvan);

                var ttrinhtruoc = tienTrinhSrv.GetbyYCau(congvan.MaYeuCau, congvan.MaCViec, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Now;
                    ttrinhtruoc.MA_CVIECTIEP = maCViec;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tienTrinhSrv.Save(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = tienTrinhSrv.GetbyYCau(congvan.MaYeuCau, maCViec, -1);
                if (tientrinh == null || (tientrinh != null && tientrinh.TRANG_THAI == 1))
                    tientrinh = new DvTienTrinh();
                tientrinh.TRANG_THAI = 0;
                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = bPhanNhan;
                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TRUOC;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_NVIEN_NHAN = nVienNhan;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
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
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.DaXuLy;
                tbao.NoiDung = $"Yêu cầu phân công khảo sát, mã yêu cầu: {congvan.MaYeuCau}, ngày yêu cầu: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = nVienNhan;
                tbao.BPhanNhan = bPhanNhan;
                tbao.CongViec = noiDung;
                service.CreateNew(tbao);
                CommitTran();

                try
                {
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == nVienNhan).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Yêu cầu khảo sát");
                            sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                            IDeliverService deliver = new DeliverService();
                            deliver.Deliver(congvan.MaYeuCau);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, tbao.NoiDung);

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                RolbackTran();
                return false;
            }
        }

        public CongVanYeuCau GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }

        public void SyncbyDate(string maLoaiYCau, DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                var result = ApiHelper.SyncApi($"/api/yeucau/filter/{maLoaiYCau}/{tuNgay.ToString("dd-MM-yyyy")}/{denNgay.ToString("dd-MM-yyyy")}");
                if (result == null)
                    return;

                var list = JsonConvert.DeserializeObject<IList<TienTiepNhanData>>(result);
                var group = list.GroupBy(p => p.MaYeuCau);
                foreach (var data in group)
                {
                    var item = data.FirstOrDefault();
                    var yeucau = GetbyMaYCau(data.Key);
                    if (yeucau == null)
                    {
                        yeucau = new CongVanYeuCau();
                        yeucau.MaCViec = "YCM";
                        if (yeucau.TinhTrang == 1)
                        {
                            yeucau.TrangThai = TrangThaiCongVan.TiepNhan;
                            yeucau.MaCViec = "TN";
                        }
                    }

                    yeucau.MaKHang = item.MaKHang;
                    yeucau.MaDDoDDien = item.MaDDoDDien;

                    yeucau.MaLoaiYeuCau = item.MaLoaiYeuCau;
                    yeucau.MaYeuCau = item.MaYeuCau;
                    yeucau.MaDViTNhan = item.MaDViTNhan;
                    yeucau.MaDViQLy = item.MaDViQLy;
                    if (!string.IsNullOrWhiteSpace(item.NguoiYeuCau))
                        yeucau.NguoiYeuCau = item.NguoiYeuCau;
                    if (!string.IsNullOrWhiteSpace(item.DChiNguoiYeuCau))
                        yeucau.DChiNguoiYeuCau = item.DChiNguoiYeuCau;
                    yeucau.DuongPho = item.DuongPho;
                    yeucau.SoNha = item.SoNha;
                    yeucau.DiaChinhID = item.DiaChinhID;

                    if (!string.IsNullOrWhiteSpace(item.TenKhachHang))
                        yeucau.TenKhachHang = item.TenKhachHang;
                    if (!string.IsNullOrWhiteSpace(item.CoQuanChuQuan))
                        yeucau.CoQuanChuQuan = item.CoQuanChuQuan;
                    if (!string.IsNullOrWhiteSpace(item.DiaChiCoQuan))
                        yeucau.DiaChiCoQuan = item.DiaChiCoQuan;
                    if (!string.IsNullOrWhiteSpace(item.MST))
                        yeucau.MST = item.MST;
                    if (!string.IsNullOrWhiteSpace(item.DienThoai))
                        yeucau.DienThoai = item.DienThoai;
                    if (!string.IsNullOrWhiteSpace(item.Email))
                        yeucau.Email = item.Email;

                    yeucau.Fax = item.Fax;
                    yeucau.SoTaiKhoan = item.SoTaiKhoan;
                    yeucau.SoPha = item.SoPha.ToString();
                    yeucau.DienSinhHoat = item.DienSinhHoat;
                    yeucau.TinhTrang = item.TinhTrang;
                    yeucau.MaHinhThuc = item.MaHinhThuc;
                    if (!string.IsNullOrWhiteSpace(item.NoiDungYeuCau))
                        yeucau.NoiDungYeuCau = item.NoiDungYeuCau;

                    if (string.IsNullOrWhiteSpace(yeucau.DiaChiDungDien))
                    {
                        yeucau.DuAnDien = $"{item.SoNha}, {item.DuongPho}";
                        yeucau.DiaChiDungDien = $"{item.SoNha}, {item.DuongPho}";
                    }
                    Save(yeucau);
                }
                CommitChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public CongVanYeuCau SyncData(string maYCau)
        {
            try
            {
                var congvan = GetbyMaYCau(maYCau);
                if (congvan == null) return null;

                var result = ApiHelper.SyncApi($"/api/yeucau/getinfo/{maYCau}");
                if (result == null)
                    return congvan;
                ICauHinhDongBoService cfgservice = IoC.Resolve<ICauHinhDongBoService>();

                var list = JsonConvert.DeserializeObject<IList<TienTiepNhanData>>(result);

                var item = list.FirstOrDefault(p => p.MaYeuCau == maYCau);
                if (item == null) return congvan;

                congvan.MaKHang = item.MaKHang;
                congvan.MaDDoDDien = item.MaDDoDDien;

                congvan.MaLoaiYeuCau = item.MaLoaiYeuCau;
                congvan.MaYeuCau = item.MaYeuCau;
                congvan.MaDViTNhan = item.MaDViTNhan;
                congvan.MaDViQLy = item.MaDViQLy;
                if (!string.IsNullOrWhiteSpace(item.NguoiYeuCau))
                    congvan.NguoiYeuCau = item.NguoiYeuCau;
                if (!string.IsNullOrWhiteSpace(item.DChiNguoiYeuCau))
                    congvan.DChiNguoiYeuCau = item.DChiNguoiYeuCau;
                congvan.DuongPho = item.DuongPho;
                congvan.SoNha = item.SoNha;
                if (!string.IsNullOrWhiteSpace(item.NoiDungYeuCau))
                    congvan.NoiDungYeuCau = item.NoiDungYeuCau;
                if (!string.IsNullOrWhiteSpace(item.DiaChinhID))
                    congvan.DiaChinhID = item.DiaChinhID;

                if (!string.IsNullOrWhiteSpace(item.TenKhachHang))
                    congvan.TenKhachHang = item.TenKhachHang;
                if (!string.IsNullOrWhiteSpace(item.CoQuanChuQuan))
                    congvan.CoQuanChuQuan = item.CoQuanChuQuan;
                if (!string.IsNullOrWhiteSpace(item.DiaChiCoQuan))
                    congvan.DiaChiCoQuan = item.DiaChiCoQuan;
                if (!string.IsNullOrWhiteSpace(item.MST))
                    congvan.MST = item.MST;
                if (!string.IsNullOrWhiteSpace(item.DienThoai))
                    congvan.DienThoai = item.DienThoai;
                if (!string.IsNullOrWhiteSpace(item.Email))
                    congvan.Email = item.Email;
                congvan.Fax = item.Fax;
                congvan.SoTaiKhoan = item.SoTaiKhoan;
                congvan.SoPha = item.SoPha.ToString();
                congvan.DienSinhHoat = item.DienSinhHoat;
                congvan.TinhTrang = item.TinhTrang;
                congvan.MaHinhThuc = item.MaHinhThuc;
                if (!string.IsNullOrWhiteSpace(item.NoiDungYeuCau))
                    congvan.NoiDungYeuCau = item.NoiDungYeuCau;

                Save(congvan);
                CommitChanges();
                return congvan;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public CongVanYeuCau DongBo(CongVanYeuCau yeucau)
        {
            try
            {
                var result = ApiHelper.PostApi("/api/yeucau/dongbo", yeucau.MaYeuCau);
                if (result == null)
                    return yeucau;
                
                var list = JsonConvert.DeserializeObject<IList<TienTiepNhanData>>(result);
                var item = list.FirstOrDefault(p => p.MaYeuCau == yeucau.MaYeuCau);
                if (item == null) return yeucau;

                yeucau.MaKHang = item.MaKHang;

                yeucau.MaLoaiYeuCau = item.MaLoaiYeuCau;
                yeucau.MaDViTNhan = item.MaDViTNhan;
                yeucau.MaDViQLy = item.MaDViQLy;
                if (!string.IsNullOrWhiteSpace(item.NguoiYeuCau))
                    yeucau.NguoiYeuCau = item.NguoiYeuCau;
                if (!string.IsNullOrWhiteSpace(item.DChiNguoiYeuCau))
                    yeucau.DChiNguoiYeuCau = item.DChiNguoiYeuCau;
                yeucau.DuongPho = item.DuongPho;
                yeucau.SoNha = item.SoNha;
                if (!string.IsNullOrWhiteSpace(item.NoiDungYeuCau))
                    yeucau.NoiDungYeuCau = item.NoiDungYeuCau;

                if (!string.IsNullOrWhiteSpace(item.DiaChinhID))
                    yeucau.DiaChinhID = item.DiaChinhID;

                if (!string.IsNullOrWhiteSpace(item.TenKhachHang))
                    yeucau.TenKhachHang = item.TenKhachHang;
                if (!string.IsNullOrWhiteSpace(item.CoQuanChuQuan))
                    yeucau.CoQuanChuQuan = item.CoQuanChuQuan;
                if (!string.IsNullOrWhiteSpace(item.DiaChiCoQuan))
                    yeucau.DiaChiCoQuan = item.DiaChiCoQuan;
                if (!string.IsNullOrWhiteSpace(item.MST))
                    yeucau.MST = item.MST;
                if (!string.IsNullOrWhiteSpace(item.DienThoai))
                    yeucau.DienThoai = item.DienThoai;
                if (!string.IsNullOrWhiteSpace(item.Email))
                    yeucau.Email = item.Email;
                yeucau.Fax = item.Fax;
                yeucau.SoTaiKhoan = item.SoTaiKhoan;
                yeucau.DienSinhHoat = item.DienSinhHoat;
                yeucau.TinhTrang = item.TinhTrang;
                yeucau.MaHinhThuc = item.MaHinhThuc;               
                
                return yeucau;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return yeucau;
            }
        }

        public bool Cancel(CongVanYeuCau yeucau)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                BeginTran();
                if (yeucau.TrangThai == TrangThaiCongVan.TiepNhan)
                {
                    long nextstep = tienTrinhSrv.LastbyMaYCau(yeucau.MaYeuCau);
                    DvTienTrinh tientrinh = new DvTienTrinh();
                    tientrinh.MA_DVIQLY = yeucau.MaDViQLy;
                    tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;

                    tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                    tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                    tientrinh.MA_BPHAN_NHAN = userdata.maBPhan;
                    tientrinh.MA_NVIEN_NHAN = userdata.maNVien;

                    tientrinh.MA_CVIEC = "HU";
                    tientrinh.MA_CVIECTIEP = "HU";

                    tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;

                    tientrinh.NDUNG_XLY = "Trả lại hồ sơ";

                    tientrinh.NGAY_BDAU = DateTime.Today;
                    tientrinh.NGAY_KTHUC = DateTime.Now;

                    tientrinh.NGAY_HEN = DateTime.Now;
                    tientrinh.SO_LAN = 1;

                    tientrinh.NGAY_TAO = DateTime.Now;
                    tientrinh.NGAY_SUA = DateTime.Now;

                    tientrinh.NGUOI_TAO = userdata.maNVien;
                    tientrinh.NGUOI_SUA = userdata.maNVien;
                    if (tientrinh.STT == 0)
                        tientrinh.STT = nextstep;

                    tienTrinhSrv.CreateNew(tientrinh);
                }

                yeucau.TrangThai = TrangThaiCongVan.Huy;
                yeucau.NgayDuyet = DateTime.Now;
                yeucau.NguoiDuyet = userdata.maNVien;
                yeucau.MaCViec = "HU";
                Save(yeucau);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.CanhBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Hủy yêu cầu: {yeucau.MaYeuCau}, người hủy: {userdata.maNVien}, ngày hủy: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = userdata.maNVien;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = yeucau.LyDoHuy;
                service.CreateNew(tbao);
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, tbao.NoiDung);

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(yeucau.MaDViQLy, yeucau.MaYeuCau);
                try
                {
                    var sendmail = new SendMail();
                    sendmail.EMAIL = yeucau.Email;
                    sendmail.TIEUDE = "Trả lại yêu cầu đề nghị đấu nối vào lưới điện trung áp";
                    sendmail.MA_DVIQLY = yeucau.MaDViQLy;
                    sendmail.MA_YCAU_KNAI = yeucau.MaYeuCau;
                    Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                    bodyParams.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                    bodyParams.Add("$maYCau", yeucau.MaYeuCau);
                    bodyParams.Add("$ngayYCau", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                    bodyParams.Add("$ngayTraLai", DateTime.Now.ToString("dd/MM/yyyy"));
                    bodyParams.Add("$lyDo", yeucau.LyDoHuy);
                    sendmailsrv.Process(sendmail, "TraLaiYeuCauDauNoi", bodyParams);
                    deliver.Deliver(yeucau.MaYeuCau);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool ChuyenTiep(string maYCau, string maDViTNhan)
        {
            try
            {
                CongVanYeuCau yeucau = GetbyMaYCau(maYCau);
                ICmisProcessService cmisProcess = new CmisProcessService();
                if (!cmisProcess.ChuyenTiep(yeucau, maDViTNhan))
                    return false;
                try
                {
                    IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                    yeucau.TrangThai = TrangThaiCongVan.ChuyenTiep;
                    Save(yeucau);

                    IList<DvTienTrinh> tientrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == maYCau).ToList();
                    foreach (var ttrinh in tientrinhs)
                    {
                        ttrinh.MA_DVIQLY = maDViTNhan;
                        ttrinhsrv.Save(ttrinh);
                    }
                    CommitChanges();
                    SyncData(maYCau);
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
                return false;
            }
        }

        /// <summary>
        /// Hủy yêu cầu khảo sát:
        /// - Cập nhật trạng thái của công văn yêu cầu về trạng thái trước đó
        /// </summary>
        public bool CancelYeuCauKhaoSat(CongVanYeuCau congvan)
        {
            try
            {
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();

                BeginTran();

                if (congvan.TrangThai > TrangThaiCongVan.TiepNhan)
                {
                    congvan.TrangThai = TrangThaiCongVan.TiepNhan;
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

        public void SyncHU()
        {
            try
            {
                ICongVanYeuCauService yeucausrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                var listHU = Query.Where(p => p.TrangThai == TrangThaiCongVan.Huy).ToList();
                foreach (var item in listHU)
                {
                    if (string.IsNullOrWhiteSpace(item.MaDDoDDien))
                    {
                        var ycau = yeucausrv.GetbyMaYCau(item.MaYeuCau);
                        item.MaDDoDDien = ycau.MaDDoDDien;
                    }
                    ttrinhsrv.DongBoTienTrinhHU(item);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
