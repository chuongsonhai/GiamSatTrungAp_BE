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
    public class BienBanDNService : FX.Data.BaseService<BienBanDN, int>, IBienBanDNService
    {
        ILog log = LogManager.GetLogger(typeof(BienBanDNService));
        public BienBanDNService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public IList<BienBanDN> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.NgayLap >= fromdate.Date && p.NgayLap < todate.Date.AddDays(1));
            if (status > -1)
                query = query.Where(p => p.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(maDViQly))
                query = query.Where(p => p.MaDViQLy == maDViQly);
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MaYeuCau == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.SoBienBan.Contains(keyword) || p.TenCongTrinh.Contains(keyword) || p.KHTen.Contains(keyword));
            total = query.Count();
            query = query.OrderByDescending(p => p.NgayLap);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public bool Save(BienBanDN bienban, out string message)
        {
            message = "";
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                var yeucau = service.GetbyMaYCau(bienban.MaYeuCau);
                AsposeUtils aspose = new AsposeUtils();
                if (string.IsNullOrWhiteSpace(bienban.Data) && !string.IsNullOrWhiteSpace(bienban.DocPath))
                {
                    string folder = $"{bienban.MaDViQLy}/{bienban.MaYeuCau}";
                    var pdfPath = aspose.ConvertWordToPDF(folder, bienban.DocPath);
                    if (!string.IsNullOrWhiteSpace(pdfPath))
                        bienban.Data = pdfPath;
                    log.Error(bienban.Data);
                }
                string maLoaiHSo = LoaiHSoCode.BB_DN;

                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                if (hoSo == null)
                {
                    hoSo = new HoSoGiayTo();
                    hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                    hoSo.TenHoSo = "Dự thảo thỏa thuận đấu nối";
                    hoSo.LoaiHoSo = maLoaiHSo;
                }

                bool sendnotify = bienban.ID == 0;
                BeginTran();
                hoSo.TrangThai = -1;
                hoSo.MaYeuCau = yeucau.MaYeuCau;
                hoSo.MaDViQLy = yeucau.MaDViQLy;
                hoSo.Data = bienban.Data;
                hsogtosrv.Save(hoSo);

                bienban.TrangThai = (int)TrangThaiBienBan.DuThao;
                Save(bienban);
                CommitTran();
                try
                {
                    if (sendnotify)
                    {
                        ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                        var sendmail = new SendMail();
                        sendmail.EMAIL = yeucau.Email;
                        sendmail.TIEUDE = "Kiểm tra, xác nhận dự thảo thỏa thuận đấu nối";
                        sendmail.MA_DVIQLY = yeucau.MaDViQLy;
                        sendmail.MA_YCAU_KNAI = yeucau.MaYeuCau;
                        Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                        bodyParams.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                        bodyParams.Add("$maYCau", yeucau.MaYeuCau);
                        bodyParams.Add("$ngayYCau", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                        bodyParams.Add("$ngayThoaThuan", bienban.NgayDuyet.ToString("dd/MM/yyyy"));
                        sendmailsrv.Process(sendmail, "BienBanDauNoi", bodyParams);

                        IDeliverService deliver = new DeliverService();
                        deliver.Deliver(yeucau.MaYeuCau);
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
                log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Gửi Biên bản đấu nối đã ký trên DO cho khách hàng xác nhận
        /// - Đồng bộ tiến trình BDN lên CMIS
        /// - Gán tiến trình hiện tại thành DDN và đồng bộ lên CMIS
        /// </summary>
        /// <param name="bienban"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Notify(BienBanDN bienban, out string message)
        {
            message = "";
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                string maCViec = "DDN";
                string maCViecTiep = "KDN";

                var congvan = service.GetbyMaYCau(bienban.MaYeuCau);
                AsposeUtils aspose = new AsposeUtils();
                if (string.IsNullOrWhiteSpace(bienban.Data) && !string.IsNullOrWhiteSpace(bienban.DocPath))
                {
                    string folder = $"{bienban.MaDViQLy}/{bienban.MaYeuCau}";
                    bienban.Data = aspose.ConvertWordToPDF(folder, bienban.DocPath);
                }
                string maLoaiHSo = LoaiHSoCode.BB_DN;
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);
                if (hoSo == null)
                {
                    hoSo = new HoSoGiayTo();
                    hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                    hoSo.TenHoSo = "Dự thảo thỏa thuận đấu nối";
                    hoSo.LoaiHoSo = maLoaiHSo;
                }

                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(bienban.MaDViQLy, bienban.MaYeuCau, new int[] { 0, 2 });
                long nextstep = tientrinhsrv.LastbyMaYCau(congvan.MaYeuCau);                

                BeginTran();
                hoSo.TrangThai = 0;
                hoSo.MaYeuCau = congvan.MaYeuCau;
                hoSo.MaDViQLy = congvan.MaDViQLy;
                hoSo.Data = bienban.Data;
                hsogtosrv.Save(hoSo);

                bienban.MaDDoDDien = congvan.MaDDoDDien;
                bienban.MaLoaiYeuCau = congvan.MaLoaiYeuCau;

                //Đồng bộ tiến trình BDN lên CMIS
                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == "BDN");
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                bienban.NgayDuyet = DateTime.Now;
                bienban.NguoiDuyet = userdata.maNVien;
                bienban.TrangThai = (int)TrangThaiBienBan.DaDuyet;
                Save(bienban);

                DvTienTrinh tientrinh = new DvTienTrinh();

                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = "EVN ký xác nhận thỏa thuận đấu nối";

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = congvan.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = userdata.maNVien;
                if (ttrinhtruoc != null)
                {
                    tientrinh.MA_BPHAN_GIAO = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_GIAO = ttrinhtruoc.MA_NVIEN_NHAN;

                    tientrinh.MA_BPHAN_NHAN = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinhtruoc.MA_NVIEN_NHAN;
                }

                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = maCViecTiep;

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Now;

                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;

                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.Save(tientrinh);

                congvan.TrangThai = TrangThaiCongVan.DuThaoTTDN;
                congvan.MaCViec = maCViec;
                service.Save(congvan);
                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, "EVN ký xác nhận thỏa thuận đấu nối");
                
                try
                {
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    var sendmail = new SendMail();
                    sendmail.EMAIL = congvan.Email;
                    sendmail.TIEUDE = "Kiểm tra, xác nhận thỏa thuận đấu nối";
                    sendmail.MA_DVIQLY = congvan.MaDViQLy;
                    sendmail.MA_YCAU_KNAI = congvan.MaYeuCau;
                    Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                    bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                    bodyParams.Add("$maYCau", congvan.MaYeuCau);
                    bodyParams.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                    bodyParams.Add("$ngayThoaThuan", bienban.NgayDuyet.ToString("dd/MM/yyyy"));
                    sendmailsrv.Process(sendmail, "BienBanDauNoi", bodyParams);

                    deliver.Deliver(congvan.MaYeuCau);
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
                log.Error(ex);
                return false;
            }
        }

        public bool Cancel(BienBanDN item)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var yeucau = cvservice.GetbyMaYCau(item.MaYeuCau);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, yeucau.MaCViec);
                var cauhinh = cauhinhs.Where(p => p.TRANG_THAI_TIEP == (int)TrangThaiCongVan.Huy).FirstOrDefault();
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(yeucau.MaDViQLy, yeucau.MaYeuCau, new int[] { 0, 2 });

                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == yeucau.MaCViec);
                BeginTran();
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = item.MaDViQLy;
                tientrinh.MA_NVIEN_GIAO = item.NguoiLap;
                tientrinh.MA_BPHAN_NHAN = item.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = item.NguoiLap;
                if (ttrinhtruoc != null)
                {
                    tientrinh.MA_BPHAN_GIAO = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_GIAO = ttrinhtruoc.MA_NVIEN_NHAN;

                    tientrinh.MA_BPHAN_NHAN = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinhtruoc.MA_NVIEN_NHAN;
                }

                tientrinh.MA_CVIEC = "HU";
                tientrinh.MA_CVIECTIEP = "HU";
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                tientrinh.NDUNG_XLY = "Hủy yêu cầu cấp điện trung áp";

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Today;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = item.NguoiLap;
                tientrinh.NGUOI_SUA = item.NguoiLap;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.Save(tientrinh);                

                yeucau.TrangThai = TrangThaiCongVan.Huy;
                item.TrangThai = 0;

                Save(item);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.CanhBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng hủy yêu cầu: {yeucau.MaYeuCau}, ngày hủy: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = item.NguoiLap;
                tbao.BPhanNhan = tientrinh.MA_BPHAN_NHAN;
                tbao.CongViec = yeucau.LyDoHuy;
                service.CreateNew(tbao);


                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var lcanhbao = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO <= 6);
                var lcanhbao1 = lcanhbao.FirstOrDefault(p => p.LOAI_CANHBAO_ID == 13);
                var checkTonTai1 = CBservice.CheckExits11(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                var check_tontai_mycau1 = CBservice.GetByMaYeuCautontai(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);

                var canhbao = new CanhBao();
                if (!checkTonTai1)
                {
                    canhbao.LOAI_CANHBAO_ID = 13;
                    canhbao.LOAI_SOLANGUI = 1;
                    canhbao.MA_YC = yeucau.MaYeuCau;
                    canhbao.THOIGIANGUI = DateTime.Now;
                    canhbao.TRANGTHAI_CANHBAO = 1;
                    canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                    canhbao.NOIDUNG = "Loại cảnh báo 13 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + yeucau.TenKhachHang + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Ngành điện gặp trở ngại gặp trở ngại trong quá trình treo tháo thiết bị đo đếm với lý do Khách hàng hủy yêu cầu: " + yeucau.MaYeuCau + ", ngày hủy:" + DateTime.Now.ToString("dd/MM/yyyy") + " , đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và khắc phục theo đúng qui định.";
                }
                else
                {
                    canhbao.LOAI_CANHBAO_ID = 13;
                    canhbao.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                    canhbao.MA_YC = yeucau.MaYeuCau;
                    canhbao.THOIGIANGUI = DateTime.Now;
                    canhbao.TRANGTHAI_CANHBAO = 1;
                    canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                    canhbao.NOIDUNG = "Loại cảnh báo 13 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + yeucau.TenKhachHang + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Ngành điện gặp trở ngại gặp trở ngại trong quá trình treo tháo thiết bị đo đếm với lý do Khách hàng hủy yêu cầu: " + yeucau.MaYeuCau + ", ngày hủy:" + DateTime.Now.ToString("dd/MM/yyyy") + " , đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và khắc phục theo đúng qui định.";
                }
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

                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(item.MaDViQLy, item.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Adjust(BienBanDN item, string noiDung)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();

                var yeucau = cvservice.GetbyMaYCau(item.MaYeuCau);

                BeginTran();
                item.TrangThai = (int)TrangThaiBienBan.DuThao;
                item.TroNgai = noiDung;
                Save(item);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.CanhBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng yêu cầu sửa nội dung: {noiDung}";
                tbao.NguoiNhan = item.NguoiLap;
                tbao.CongViec = yeucau.LyDoHuy;
                service.CreateNew(tbao);

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

        /// <summary>
        /// Xác nhận TTDN từ web CSKH
        /// - Tạo ra tiến trình KDN đồng bộ lên CMIS        
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pdfdata"></param>
        /// <returns></returns>
        public bool Confirm(BienBanDN item, byte[] pdfdata)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();

                var yeucau = cvservice.GetbyMaYCau(item.MaYeuCau);

                string maCViec = "KDN";
                string maCViecTiep = "TVB";

                string maLoaiHSo = LoaiHSoCode.BB_DN;

                var hoso = hsoservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, "DDN", 0);
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                
                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(yeucau.MaDViQLy, yeucau.MaYeuCau, new int[] { 0, 2 });
                BeginTran();
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.TRANG_THAI = 2;
                    ttrinhtruoc.MA_CVIECTIEP = "KDN";
                    tientrinhsrv.Save(ttrinhtruoc);
                    tientrinhs.Add(ttrinhtruoc);
                }
                IRepository repository = new FileStoreRepository();
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";

                item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;
                item.Data = repository.Store(folder, pdfdata, item.Data);
                Save(item);

                hoso.Data = item.Data;
                hoso.TrangThai = 2;
                hsoservice.Save(hoso);

                yeucau.TrangThai = TrangThaiCongVan.DuChuKy;
                yeucau.MaCViec = maCViecTiep;
                cvservice.Save(yeucau);

                DvTienTrinh tientrinh = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == maCViec && p.TRANG_THAI == 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = item.MaDViQLy;
                tientrinh.MA_NVIEN_GIAO = item.NguoiLap;
                tientrinh.MA_BPHAN_NHAN = item.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = item.NguoiLap;
                if (ttrinhtruoc != null)
                {
                    tientrinh.MA_BPHAN_GIAO = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_GIAO = ttrinhtruoc.MA_NVIEN_NHAN;

                    tientrinh.MA_BPHAN_NHAN = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinhtruoc.MA_NVIEN_NHAN;
                }

                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = maCViecTiep;
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                tientrinh.NDUNG_XLY = "Khách hàng xác nhận thỏa thuận đấu nối";

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Today;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = item.NguoiLap;
                tientrinh.NGUOI_SUA = item.NguoiLap;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.Save(tientrinh);                

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng đã ký thỏa thuận đấu nối, ngày ký: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = item.NguoiLap;
                tbao.BPhanNhan = tientrinh.MA_BPHAN_NHAN;
                tbao.CongViec = tientrinh.NDUNG_XLY;
                service.CreateNew(tbao);

                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(item.MaDViQLy, item.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Complete(BienBanDN item, string maPBanNhan, string nVienNhan, DateTime ngayHen)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                var pdfdata = repository.GetData(item.Data);
                if (pdfdata == null)
                    throw new Exception("Không tìm thấy file path.");

                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();

                var yeucau = service.GetbyMaYCau(item.MaYeuCau);

                yeucau.TrangThai = TrangThaiCongVan.HoanThanh;
                yeucau.MaCViec = "KDN";
                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;

                string maLoaiHSo = LoaiHSoCode.BB_DN;
                if (item.TrangThai == (int)TrangThaiBienBan.HoanThanh)
                {
                    ICmisProcessService cmisdeliver = new CmisProcessService();
                    if (!cmisdeliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, pdfdata, maLoaiHSo))
                    {
                        log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                        //return false;
                    }
                }

                BeginTran();
                service.Save(yeucau);
                Save(item);                
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

        public BienBanDN GetbyNo(string sobienban, string mayeucau)
        {
            return Get(p => p.SoBienBan == sobienban && p.MaYeuCau == mayeucau);
        }

        public BienBanDN GetbyMaYeuCau(string maYeuCau)
        {
            return Get(p => p.MaYeuCau == maYeuCau);
        }
    }
}