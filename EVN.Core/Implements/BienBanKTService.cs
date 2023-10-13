using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.PMIS;
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
    public class BienBanKTService : FX.Data.BaseService<BienBanKT, int>, IBienBanKTService
    {
        ILog log = LogManager.GetLogger(typeof(BienBanKTService));
        public BienBanKTService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public BienBanKT Update(BienBanKT item, BienBanDN thoathuandn, IList<ThanhPhanKT> thanhPhans)
        {
            try
            {
                IThanhPhanKTService tpservice = IoC.Resolve<IThanhPhanKTService>();
                INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
                NoTransaction notran = new NoTransaction(notempsrv, NoType.BBKTDongDien);

                BeginTran();
                if (string.IsNullOrWhiteSpace(item.SoBienBan))
                {
                    item.SoBienBan = thoathuandn.MaYeuCau;
                    //lock (notran)
                    //{
                    //    decimal no = notran.GetNextNo();
                    //    item.SoBienBan = notran.GetCode(item.MaDViQLy, no);
                    //}
                }

                foreach (var tphan in item.ThanhPhans)
                {
                    tpservice.Delete(tphan);
                }

                item.ThoaThuanID = thoathuandn.ID;
                item.SoThoaThuan = thoathuandn.SoBienBan;
                item.NgayThoaThuan = thoathuandn.NgayLap;
                item.MaYeuCau = thoathuandn.MaYeuCau;
                item.MaDViQLy = thoathuandn.MaDViQLy;
                item.MaSoThue = thoathuandn.KHMaSoThue;
                item.ThanhPhans = thanhPhans;
                if (item.ThuanLoi)
                {
                    item.Data = item.GetPdf(true);
                }
                Save(item);

                foreach (var tphan in thanhPhans)
                {
                    tphan.BienBanID = item.ID;
                    tpservice.CreateNew(tphan);
                }

                notran.CommitTran();
                CommitTran();
                return item;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return null;
            }
        }

        public BienBanKT GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }

        public bool Approve(BienBanKT item, KetQuaKT ketqua)
        {
            try
            {
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaKTService kqservice = IoC.Resolve<IKetQuaKTService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IPhanCongTCService phanCongKTService = IoC.Resolve<IPhanCongTCService>();

                string maLoaiHSo = LoaiHSoCode.BB_KT;
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                HoSoGiayTo hoSo = hsoservice.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                if (hoSo == null)
                {
                    hoSo = new HoSoGiayTo();
                    hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                    hoSo.TenHoSo = "Biên bản kiểm tra điều kiện đóng điện điểm đấu nối";
                }

                var cviechtai = cauhinhsrv.Get(p => p.MA_LOAI_YCAU == yeucau.MaLoaiYeuCau && p.MA_CVIEC_TRUOC == ketqua.MA_CVIEC_TRUOC && p.MA_CVIEC_TIEP == ketqua.MA_CVIEC);
                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, ketqua.MA_CVIEC);
                var cauhinh = cauhinhs.FirstOrDefault();

                if (ketqua.MA_CVIEC_TRUOC == ketqua.MA_CVIEC) cauhinh = cviechtai;
                long nextstep = tientrinhsrv.LastbyMaYCau(item.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                BeginTran();
                if (item.ThuanLoi)
                {
                    hoSo.TrangThai = 0;
                    hoSo.MaYeuCau = yeucau.MaYeuCau;
                    hoSo.MaDViQLy = yeucau.MaDViQLy;
                    hoSo.LoaiHoSo = maLoaiHSo;
                    hoSo.Data = item.Data;
                    hsoservice.Save(hoSo);
                    item.TroNgai = String.Empty;
                }

                ketqua.TRANG_THAI = 1;
                item.TrangThai = (int)TrangThaiBienBan.DaDuyet;
                yeucau.TrangThai = TrangThaiNghiemThu.BienBanKT;
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(ketqua.MA_YCAU_KNAI, ketqua.MA_CVIEC_TRUOC, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = ketqua.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }
                var dalaphd = tientrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "DTN" && p.TRANG_THAI == 1);

                //Nếu chưa lập hợp đồng, hoặc có trở ngại
                if (!dalaphd || (ketqua.MA_CVIEC != "DTN" && ketqua.MA_CVIEC != "KHD"))
                {
                    DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, ketqua.MA_CVIEC, 0);
                    if (tientrinh == null)
                        tientrinh = new DvTienTrinh();
                    tientrinh.TRANG_THAI = 0;
                    tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                    tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                    tientrinh.MA_BPHAN_NHAN = ketqua.MA_BPHAN_NHAN;
                    tientrinh.MA_CVIEC = ketqua.MA_CVIEC;
                    tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                    tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                    tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                    tientrinh.MA_NVIEN_NHAN = ketqua.MA_NVIEN_NHAN;
                    tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;

                    tientrinh.NDUNG_XLY = ketqua.NDUNG_XLY;

                    tientrinh.NGAY_BDAU = DateTime.Today;

                    tientrinh.NGAY_KTHUC = DateTime.Now;

                    tientrinh.NGAY_HEN = ketqua.NGAY_HEN;
                    tientrinh.SO_LAN = 1;

                    tientrinh.NGAY_TAO = DateTime.Now;
                    tientrinh.NGAY_SUA = DateTime.Now;

                    tientrinh.NGUOI_TAO = userdata.maNVien;
                    tientrinh.NGUOI_SUA = userdata.maNVien;
                    if (tientrinh.STT == 0)
                        tientrinh.STT = nextstep;

                    tientrinhsrv.Save(tientrinh);
                }

                if (cviechtai != null && cviechtai.TRANG_THAI_TIEP.HasValue)
                {
                    ketqua.TRANG_THAI = 0;
                    ketqua.THUAN_LOI = false;

                    item.ThuanLoi = false;
                    item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                    yeucau.MaCViec = cauhinh.MA_CVIEC_TIEP;
                    yeucau.TrangThai = (TrangThaiNghiemThu)cviechtai.TRANG_THAI_TIEP.Value;

                    if (yeucau.TrangThai == TrangThaiNghiemThu.TiepNhan)
                    {
                        var thongbao = tbservice.GetbyYCau(item.MaDViQLy, item.MaYeuCau, LoaiThongBao.KiemTra);
                        if (thongbao != null)
                        {
                            tbservice.Delete(thongbao);
                        }
                        var pc = phanCongKTService.GetbyMaYCau(yeucau.MaLoaiYeuCau, yeucau.MaYeuCau, 0);
                        if (pc != null)
                        {
                            pc.TRANG_THAI = 0;
                            phanCongKTService.Save(pc);
                        }
                    }

                    DvTienTrinh ttrinhtngai = new DvTienTrinh();
                    ttrinhtngai.MA_BPHAN_GIAO = userdata.maBPhan;
                    ttrinhtngai.MA_NVIEN_GIAO = userdata.maNVien;

                    ttrinhtngai.MA_BPHAN_NHAN = ketqua.MA_BPHAN_NHAN;
                    ttrinhtngai.MA_CVIEC = cauhinh.MA_CVIEC_TIEP;
                    ttrinhtngai.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                    ttrinhtngai.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                    ttrinhtngai.MA_DVIQLY = yeucau.MaDViQLy;

                    ttrinhtngai.MA_NVIEN_NHAN = ketqua.MA_NVIEN_NHAN;
                    ttrinhtngai.MA_YCAU_KNAI = yeucau.MaYeuCau;

                    ttrinhtngai.MA_TNGAI = ketqua.MA_TNGAI;
                    ttrinhtngai.NDUNG_XLY = ketqua.NDUNG_XLY;
                    if (!string.IsNullOrWhiteSpace(item.TroNgai))
                        ttrinhtngai.NDUNG_XLY = item.TroNgai;

                    ttrinhtngai.NGAY_BDAU = DateTime.Today;                    
                    ttrinhtngai.NGAY_KTHUC = DateTime.Now;

                    ttrinhtngai.NGAY_HEN = ketqua.NGAY_HEN;
                    ttrinhtngai.SO_LAN = 1;

                    ttrinhtngai.NGAY_TAO = DateTime.Now;
                    ttrinhtngai.NGAY_SUA = DateTime.Now;

                    ttrinhtngai.NGUOI_TAO = userdata.maNVien;
                    ttrinhtngai.NGUOI_SUA = userdata.maNVien;
                    ttrinhtngai.STT = nextstep + 1;

                    tientrinhsrv.Save(ttrinhtngai);

                    ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                    var lcanhbao = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO <= 6);
                    var lcanhbao1 = lcanhbao.FirstOrDefault(p => p.LOAI_CANHBAO_ID == 14);
                    var checkTonTai1 = CBservice.CheckExits11(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                    var check_tontai_mycau1 = CBservice.GetByMaYeuCautontai(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);

                    var canhbao = new CanhBao();
                    if (!checkTonTai1)
                    {
                        canhbao.LOAI_CANHBAO_ID = 14;
                        canhbao.LOAI_SOLANGUI = 1;
                        canhbao.MA_YC = yeucau.MaYeuCau;
                        canhbao.THOIGIANGUI = DateTime.Now;
                        canhbao.TRANGTHAI_CANHBAO = 1;
                        canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                        canhbao.NOIDUNG = "Loại cảnh báo 14 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + hoSo.KHXacNhan + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Khách hàng từ chối ký HĐMBĐ với lý do " + ketqua.NDUNG_XLY + ", đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và liên hệ với khách hàng để xử lý đúng qui định";
                    }
                    else
                    {
                        canhbao.LOAI_CANHBAO_ID = 14;
                        canhbao.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                        canhbao.MA_YC = yeucau.MaYeuCau;
                        canhbao.THOIGIANGUI = DateTime.Now;
                        canhbao.TRANGTHAI_CANHBAO = 1;
                        canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                        canhbao.NOIDUNG = "Loại cảnh báo 14 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + hoSo.KHXacNhan + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Khách hàng từ chối ký HĐMBĐ với lý do " + ketqua.NDUNG_XLY + ", đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và liên hệ với khách hàng để xử lý đúng qui định";
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
                }

                kqservice.Save(ketqua);
                Save(item);
                service.Save(yeucau);
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, "Lập BBKT");

                try
                {
                    IDeliverService deliver = new DeliverService();
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    if (item.ThuanLoi)
                    {
                        var sendmail = new SendMail();
                        sendmail.EMAIL = yeucau.Email;
                        sendmail.TIEUDE = "Kiểm tra, xác nhận biên bản kiểm tra điều kiện đóng điện điểm đấu nối";
                        sendmail.MA_DVIQLY = yeucau.MaDViQLy;
                        sendmail.MA_YCAU_KNAI = yeucau.MaYeuCau;
                        Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                        bodyParams.Add("$khachHang", item.KHTen ?? yeucau.NguoiYeuCau);
                        bodyParams.Add("$soThoaThuan", yeucau.SoThoaThuanDN);
                        bodyParams.Add("$ngayKhaoSat", item.NgayLap.ToString("dd/MM/yyyy"));
                        sendmailsrv.Process(sendmail, "BienBanKiemTra", bodyParams);
                    }
                    else
                    {
                        var ttrinhks = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, "KTR", -1);
                        var nvPhanCong = userdatasrv.Query.Where(x => x.maNVien == ttrinhks.MA_NVIEN_NHAN).FirstOrDefault();
                        if (nvPhanCong != null)
                        {
                            if (!string.IsNullOrWhiteSpace(nvPhanCong.email))
                            {
                                var sendmailnv = new SendMail();
                                sendmailnv.EMAIL = nvPhanCong.email;
                                sendmailnv.TIEUDE = "Thông báo về kiểm tra có trở ngại";
                                sendmailnv.MA_DVIQLY = item.MaDViQLy;
                                sendmailnv.MA_YCAU_KNAI = item.MaYeuCau;
                                Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                                bodyParamsNV.Add("$khachHang", yeucau.NguoiYeuCau);
                                bodyParamsNV.Add("$donVi", yeucau.MaDViQLy);
                                bodyParamsNV.Add("$duAnDien", yeucau.DuAnDien);
                                bodyParamsNV.Add("$khuVuc", yeucau.DiaChiDungDien);
                                bodyParamsNV.Add("$maYCau", yeucau.MaYeuCau);
                                bodyParamsNV.Add("$ngaytiepnhan", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                                bodyParamsNV.Add("$nhanvien", nvPhanCong.fullName ?? nvPhanCong.username);
                                bodyParamsNV.Add("$lydo", item.TroNgai);
                                sendmailsrv.Process(sendmailnv, "CanhBaoNVTroNgai", bodyParamsNV);                                
                            }
                        }
                    }
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

        public bool Sign(BienBanKT item)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                var pdfdata = repository.GetData(item.Data);
                if (pdfdata == null)
                    throw new Exception("Không tìm thấy file path.");
                IOrganizationService orgSrv = IoC.Resolve<IOrganizationService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();

                string maLoaiHSo = LoaiHSoCode.BB_KT;
                var template = TemplateManagement.GetTemplate(maLoaiHSo);
                string signPosition = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "TỔNG GIÁM ĐỐC/GIÁM ĐỐC";

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var org = orgSrv.GetbyCode(item.MaDViQLy);
                string orgCode = org.compCode;
                string signName = org.daiDien;

                var result = PdfSignUtil.SignPdf(signName, orgCode, pdfdata, signPosition);
                if (result == null)
                {
                    log.Error("Lỗi tích hợp HSM");
                    return false;
                }
                if (!result.suc)
                {
                    log.Error(result.msg);
                    return false;
                }

                ICmisProcessService deliver = new CmisProcessService();
                var data = Convert.FromBase64String(result.data);
                if (!deliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, pdfdata, maLoaiHSo))
                {
                    log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                    //return false;
                }
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, data, item.Data);

                var congvan = service.GetbyMaYCau(item.MaYeuCau);
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);

                BeginTran();
                hoSo.TrangThai = 2;
                hoSo.Data = item.Data;
                hsogtosrv.Save(hoSo);

                Save(item);
                congvan.TrangThai = TrangThaiNghiemThu.DuThaoHD;
                service.Save(congvan);

                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
                Save(item);

                try
                {
                    IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                    dataLoggingService.CreateLog(congvan.MaDViQLy, congvan.MaYeuCau, userdata.maNVien, userdata.fullName, congvan.MaCViec, "Ký BBKT");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                CommitTran();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiNghiemThu.DuThaoHD, 1);
                if (tthaiycau != null)
                {
                    ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                    IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                    var ttrinh = tientrinhsrv.GetbyYCau(item.MaYeuCau, tthaiycau.CVIEC_TRUOC, 0);
                    if (ttrinh != null && ttrinh.TRANG_THAI != 1)
                    {
                        var cviec = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, tthaiycau.CVIEC_TRUOC).FirstOrDefault();
                        ttrinh.NGAY_KTHUC = DateTime.Today;
                        ttrinh.MA_CVIECTIEP = cviec.MA_CVIEC_TIEP;

                        string message = "";
                        tientrinhsrv.PushToCmis(new List<DvTienTrinh>() { ttrinh }, out message);
                    }
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

        public IList<BienBanKT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
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

        public bool HuyKetQua(BienBanKT bienban, KetQuaKT ketqua)
        {
            try
            {
                IYCauNghiemThuService ycausrv = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaKTService ketquasrv = IoC.Resolve<IKetQuaKTService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();

                var yeucau = ycausrv.GetbyMaYCau(ketqua.MA_YCAU_KNAI);
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiNghiemThu.GhiNhanKT, 1);
                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, ketqua.MA_CVIEC);
                var cauhinh = cauhinhs.FirstOrDefault();
                var cviechtai = cauhinhsrv.Get(p => p.MA_LOAI_YCAU == yeucau.MaLoaiYeuCau && p.MA_CVIEC_TRUOC == ketqua.MA_CVIEC_TRUOC && p.MA_CVIEC_TIEP == ketqua.MA_CVIEC);
                if (ketqua.MA_CVIEC_TRUOC == ketqua.MA_CVIEC) cauhinh = cviechtai;

                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                BeginTran();
                yeucau.TrangThai = TrangThaiNghiemThu.GhiNhanKT;
                ketqua.TRANG_THAI = 0;
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, tthaiycau.CVIEC_TRUOC, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = ketqua.MA_CVIEC_TRUOC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }
                if (!string.IsNullOrWhiteSpace(ketqua.MA_TNGAI) && cviechtai != null && cviechtai.TRANG_THAI_TIEP.HasValue)
                {
                    yeucau.MaCViec = cauhinh.MA_CVIEC_TIEP;
                    yeucau.TrangThai = (TrangThaiNghiemThu)cauhinh.TRANG_THAI_TIEP.Value;

                    DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, ketqua.MA_CVIEC, 0);
                    if (tientrinh == null)
                        tientrinh = new DvTienTrinh();
                    tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                    tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                    tientrinh.MA_BPHAN_NHAN = ketqua.MA_BPHAN_NHAN;
                    tientrinh.MA_CVIEC = ketqua.MA_CVIEC;
                    tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                    tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                    tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                    tientrinh.MA_NVIEN_NHAN = ketqua.MA_NVIEN_NHAN;
                    tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;

                    tientrinh.MA_TNGAI = ketqua.MA_TNGAI;
                    tientrinh.NDUNG_XLY = ketqua.NDUNG_XLY;

                    tientrinh.NGAY_BDAU = DateTime.Today;
                    if (ttrinhtruoc != null)
                        tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                    tientrinh.NGAY_KTHUC = DateTime.Now;

                    tientrinh.NGAY_HEN = ketqua.NGAY_HEN;
                    tientrinh.SO_LAN = 1;

                    tientrinh.NGAY_TAO = DateTime.Now;
                    tientrinh.NGAY_SUA = DateTime.Now;

                    tientrinh.NGUOI_TAO = userdata.maNVien;
                    tientrinh.NGUOI_SUA = userdata.maNVien;
                    if (tientrinh.STT == 0)
                        tientrinh.STT = nextstep;

                    tientrinhsrv.Save(tientrinh);
                }
                if (yeucau.TrangThai == TrangThaiNghiemThu.PhanCongKT)
                {
                    ThongBao tbao = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.KiemTra);
                    if (tbao == null) tbao = new ThongBao();
                    tbao.MaYeuCau = yeucau.MaYeuCau;
                    tbao.MaDViQLy = yeucau.MaDViQLy;
                    tbao.NgayHen = DateTime.Now;
                    tbao.MaCViec = yeucau.MaCViec;
                    tbao.Loai = LoaiThongBao.KiemTra;
                    tbao.TrangThai = TThaiThongBao.Moi;
                    tbao.NoiDung = $"Yêu cầu thực hiện khảo sát lưới điện, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {tbao.NgayHen.ToString("dd/MM/yyyy")}";
                    tbao.NguoiNhan = !string.IsNullOrWhiteSpace(tbao.NguoiNhan) ? tbao.NguoiNhan : ketqua.MA_NVIEN_GIAO;
                    tbao.BPhanNhan = ketqua.MA_BPHAN_GIAO;
                    tbao.CongViec = bienban.TroNgai;
                    tbservice.Save(tbao);
                }
                ketquasrv.Save(ketqua);
                ycausrv.Save(yeucau);
                if (bienban != null)
                {
                    bienban.TrangThai = (int)TrangThaiBienBan.MoiTao;
                    Save(bienban);
                }
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, "Huỷ BBKT");
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Cancel(YCauNghiemThu yeucau, BienBanKT item, string noiDung)
        {
            try
            {
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IDvTienTrinhService tienTrinhSrv = IoC.Resolve<IDvTienTrinhService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                long nextstep = tienTrinhSrv.LastbyMaYCau(yeucau.MaYeuCau);
                BeginTran();
                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng hủy yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối: {yeucau.MaYeuCau}, ngày hủy: {DateTime.Now}";
                tbao.NguoiNhan = yeucau.NguoiLap;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = noiDung;
                tbservice.CreateNew(tbao);

                yeucau.TrangThai = TrangThaiNghiemThu.Huy;
                service.Save(yeucau);

                item.TrangThai = 0;

                Save(item);
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

                tientrinh.NDUNG_XLY = "Khách hàng hủy yêu cầu";

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
                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(yeucau.MaDViQLy, yeucau.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return false;
            }
        }

        public bool Confirm(BienBanKT item, byte[] pdfdata)
        {
            try
            {
                string maLoaiHSo = LoaiHSoCode.BB_KT;
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);

                IRepository repository = new FileStoreRepository();
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";

                BeginTran();
                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = item.MaYeuCau;
                tbao.MaDViQLy = item.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = item.MaCViec;
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.DaXuLy;
                tbao.NoiDung = $"Khách hàng xác nhận biên bản kiểm tra điều kiện đóng điện điểm đấu nối: {item.MaYeuCau}, ngày xác nhận: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = item.NguoiLap;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = "Thực hiện ký biên bản kiểm tra điều kiện đóng điện điểm đấu nối";
                tbservice.CreateNew(tbao);

                item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;
                item.Data = repository.Store(folder, pdfdata, item.Data);
                if (hoso == null)
                {
                    hoso = new HoSoGiayTo();
                    hoso.MaHoSo = Guid.NewGuid().ToString("N");
                    hoso.TenHoSo = "Biên bản kiểm tra điều kiện đóng điện điểm đấu nối";
                    hoso.MaYeuCau = item.MaYeuCau;
                    hoso.MaDViQLy = item.MaDViQLy;
                    hoso.LoaiHoSo = maLoaiHSo;
                }
                hoso.Data = item.Data;
                hoso.TrangThai = 1;
                hsoservice.Save(hoso);
                Save(item);
                CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return false;
            }
        }

        public bool SignRemote(BienBanKT item, byte[] pdfdata)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
                var tbservice = IoC.Resolve<IThongBaoService>();
                var ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                string maLoaiHSo = LoaiHSoCode.BB_KT;
                var template = TemplateManagement.GetTemplate(maLoaiHSo);
                string signPosition = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "NGƯỜI KIỂM TRA";
                var congvan = service.GetbyMaYCau(item.MaYeuCau);

                ttrinhsrv.DongBoTienDo(congvan);

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);

                ICmisProcessService cmisProcess = new CmisProcessService();
                if (!cmisProcess.UploadPdf(item.MaDViQLy, item.MaYeuCau, pdfdata, maLoaiHSo))
                {
                    log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                    //return false;
                }
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, pdfdata, item.Data);                

                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);

                BeginTran();
                hoSo.TrangThai = 2;
                hoSo.Data = item.Data;
                hsogtosrv.Save(hoSo);

                Save(item);
                congvan.TrangThai = TrangThaiNghiemThu.DuThaoHD;
                //var dalaphd = ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "DHD" && p.TRANG_THAI == 1);
                //if (dalaphd)
                //    congvan.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                service.Save(congvan);

                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
                Save(item);
                ThongBao tbao = tbservice.GetbyYCau(item.MaDViQLy, item.MaYeuCau, LoaiThongBao.KiemTra);
                if (tbao != null && (tbao.TrangThai == TThaiThongBao.Moi || tbao.TrangThai == TThaiThongBao.QuaHan))
                {
                    tbao.TrangThai = TThaiThongBao.DaXuLy;
                    tbservice.Save(tbao);
                }
                CommitTran();

                var tientrinhs = ttrinhsrv.ListNew(congvan.MaDViQLy, congvan.MaYeuCau, new int[] { 0, 2 });
                if (tientrinhs.Any(p => p.MA_CVIEC == "DTN"))
                    tientrinhs = tientrinhs.Where(p => p.MA_CVIEC != "DTN").ToList();                
                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(tientrinhs);

                try
                {
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    IMailCanhBaoTCTService mailCanhBaoTCTService = IoC.Resolve<IMailCanhBaoTCTService>();
                    var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);
                    if (bbks != null)
                    {
                        if (bbks.TongCongSuat >= 10000 && item.ListCongSuat.Count > 0)
                        {
                            var listmail = mailCanhBaoTCTService.GetAll();
                            foreach (var mail in listmail)
                            {
                                var sendmailTCT = new SendMail();
                                sendmailTCT.EMAIL = mail.EMAIL;
                                sendmailTCT.TIEUDE = "Cảnh báo yêu cầu thuộc thoả thuận có công suất lớn hơn 10000kvA";
                                sendmailTCT.MA_DVIQLY = item.MaDViQLy;
                                sendmailTCT.MA_YCAU_KNAI = congvan.MaYeuCau;
                                Dictionary<string, string> bodyParamsTCT = new Dictionary<string, string>();
                                bodyParamsTCT.Add("$khachHang", congvan.NguoiYeuCau);
                                bodyParamsTCT.Add("$donVi", congvan.MaDViQLy);
                                bodyParamsTCT.Add("$duAnDien", congvan.DuAnDien);
                                bodyParamsTCT.Add("$khuVuc", congvan.DiaChiDungDien);
                                bodyParamsTCT.Add("$maYCau", congvan.MaYeuCau);
                                bodyParamsTCT.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                                string congSuat = "";
                                foreach(var cs in item.ListCongSuat)
                                {
                                    congSuat = congSuat + cs + "; ";
                                }
                                bodyParamsTCT.Add("$congsuat", congSuat);
                                sendmailsrv.Process(sendmailTCT, "CanhBaoCSBBKT", bodyParamsTCT);
                            }
                            deliver.Deliver(item.MaYeuCau);
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
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Yêu cầu kiểm tra lại:
        /// - Cập nhật trạng thái của kết quả kiểm tra về = 0
        /// - Cập nhật trạng thái của biên bản kiểm tra về = 0
        /// - Cập nhật trạng thái của công văn yêu cầu nghiệm thu về trạng thái trước đó
        /// </summary>
        public bool KiemTraLai(BienBanKT item, out string message)
        {
            message = "";
            try
            {
                string maLoaiHSo = LoaiHSoCode.BB_KT;
                IKetQuaKTService ketquasrv = IoC.Resolve<IKetQuaKTService>();
                IYCauNghiemThuService ycausrv = IoC.Resolve<IYCauNghiemThuService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = ycausrv.GetbyMaYCau(item.MaYeuCau);

                if (yeucau.TrangThai > TrangThaiNghiemThu.DuThaoHD)
                {
                    message = "Không được hủy yêu cầu này.";
                    return false;
                }

                var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                var ketquaks = ketquasrv.GetbyMaYCau(yeucau.MaYeuCau);

                BeginTran();
                if (hoso != null)
                {
                    hsoservice.Delete(hoso);
                }
                ketquaks.TRANG_THAI = 0;
                ketquasrv.Save(ketquaks);

                item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                Save(item);

                yeucau.TrangThai = TrangThaiNghiemThu.GhiNhanKT;
                ycausrv.Save(yeucau);

                ThongBao tbao = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.KiemTra);
                if (tbao == null) tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.NoiDung = $"Kiểm tra điều kiện đóng điện điểm đấu nối, ngày hẹn: {tbao.NgayHen.ToString("dd/MM/yyyy")}"; ;
                tbao.NguoiNhan = !string.IsNullOrWhiteSpace(tbao.NguoiNhan) ? tbao.NguoiNhan : ketquaks.MA_NVIEN_GIAO;
                tbao.BPhanNhan = ketquaks.MA_BPHAN_GIAO;
                tbao.CongViec = item.TroNgai;
                tbservice.Save(tbao);

                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, $"Yêu cầu thực hiện kiểm tra điều kiện đóng điện điểm đấu nối, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {DateTime.Now.ToString("dd/MM/yyyy")}");
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                message = ex.Message;
                return false;
            }
        }
    }
}
