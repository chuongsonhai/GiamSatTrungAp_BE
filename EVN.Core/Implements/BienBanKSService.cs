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
    public class BienBanKSService : FX.Data.BaseService<BienBanKS, int>, IBienBanKSService
    {
        ILog log = LogManager.GetLogger(typeof(BienBanKSService));
        public BienBanKSService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool Sign(BienBanKS item, string maCViec, string maPBanNhan, string nVienNhan, DateTime ngayHen, string noiDung)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                var pdfdata = repository.GetData(item.Data);
                if (pdfdata == null)
                    throw new Exception("Không tìm thấy file path.");

                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IOrganizationService orgSrv = IoC.Resolve<IOrganizationService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();

                string maLoaiHSo = LoaiHSoCode.BB_KS;
                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                var template = TemplateManagement.GetTemplate(maLoaiHSo);

                string signPosition = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "BAN KỸ THUẬT/ PHÒNG KỸ THUẬT";

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
                var data = Convert.FromBase64String(result.data);

                ICmisProcessService cmisdeliver = new CmisProcessService();
                if (!cmisdeliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, data, maLoaiHSo))
                {
                    log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                    //return false;
                }

                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.BienBanKS, 0);

                string maCViecTruoc = tthaiycau.CVIEC_TRUOC;

                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                var ttrinhtruoc = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, maCViecTruoc, 0);

                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, data, item.Data);
                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;

                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                BeginTran();

                yeucau.TrangThai = TrangThaiCongVan.DuThaoTTDN;
                yeucau.MaCViec = maCViec;
                service.Save(yeucau);

                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.MA_CVIECTIEP = maCViec;
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                Save(item);
                hoSo.TrangThai = 2;
                hoSo.Data = item.Data;
                hsogtosrv.Save(hoSo);

                DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, maCViec, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = maPBanNhan;
                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = nVienNhan;
                tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
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

                tientrinhsrv.Save(tientrinh);
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, "Ký BBKS");

                IDeliverService deliver = new DeliverService();
                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IMailCanhBaoTCTService mailCanhBaoTCTService = IoC.Resolve<IMailCanhBaoTCTService>();
                try
                {
                    if (item.TongCongSuat >= 10000)
                    {
                        var sendmailTCT = new SendMail();
                        var listmail = mailCanhBaoTCTService.GetAll();
                        foreach (var mail in listmail)
                        {
                            sendmailTCT.EMAIL = mail.EMAIL;
                            sendmailTCT.TIEUDE = "Cảnh báo yêu cầu có công suất lớn hơn 10000kvA";
                            sendmailTCT.MA_DVIQLY = yeucau.MaDViQLy;
                            sendmailTCT.MA_YCAU_KNAI = yeucau.MaYeuCau;
                            Dictionary<string, string> bodyParamsTCT = new Dictionary<string, string>();
                            bodyParamsTCT.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                            bodyParamsTCT.Add("$maYCau", yeucau.MaYeuCau);
                            bodyParamsTCT.Add("$ngayYCau", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsTCT.Add("$congsuat", item.TongCongSuat.ToString());
                            sendmailsrv.Process(sendmailTCT, "CanhBaoCS", bodyParamsTCT);
                        }
                        deliver.Deliver(item.MaYeuCau);
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

        public bool SignRemote(BienBanKS item, byte[] pdfdata, string maCViec, string maPBanNhan, string nVienNhan, DateTime ngayHen, string noiDung)
        {
            try
            {
                IRepository repository = new FileStoreRepository();
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                IOrganizationService orgSrv = IoC.Resolve<IOrganizationService>();
                var tbservice = IoC.Resolve<IThongBaoService>();

                string maLoaiHSo = LoaiHSoCode.BB_KS;
                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                var template = TemplateManagement.GetTemplate(maLoaiHSo);

                string signPosition = template != null && !string.IsNullOrWhiteSpace(template.ChucVuKy) ? template.ChucVuKy : "BAN KỸ THUẬT/ PHÒNG KỸ THUẬT";

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                ICmisProcessService cmisdeliver = new CmisProcessService();
                if (!cmisdeliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, pdfdata, maLoaiHSo))
                {
                    log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                    //return false;
                }

                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.BienBanKS, 0);
                if (string.IsNullOrWhiteSpace(maCViec))
                    maCViec = tthaiycau.CVIEC;

                string maCViecTruoc = tthaiycau.CVIEC_TRUOC;

                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, pdfdata, item.Data);
                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;

                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                BeginTran();

                yeucau.TrangThai = TrangThaiCongVan.DuThaoTTDN;
                yeucau.MaCViec = maCViec;
                service.Save(yeucau);

                var ttrinhtruoc = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, maCViecTruoc, 0);
                if (ttrinhtruoc != null)
                {
                    ttrinhtruoc.MA_CVIECTIEP = maCViec;
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                Save(item);
                hoSo.TrangThai = 2;
                hoSo.Data = item.Data;
                hsogtosrv.Save(hoSo);

                DvTienTrinh tientrinh = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, maCViec, 0);
                if (tientrinh == null)
                    tientrinh = new DvTienTrinh();

                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = maPBanNhan;
                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = nVienNhan;
                tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = DateTime.Today;
                //if (ttrinhtruoc != null)
                //    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;
                tientrinh.NGAY_HEN = ngayHen;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.Save(tientrinh);

                ThongBao tbao = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.KhaoSat);
                if (tbao != null && (tbao.TrangThai == TThaiThongBao.Moi || tbao.TrangThai == TThaiThongBao.QuaHan))
                {
                    tbao.TrangThai = TThaiThongBao.DaXuLy;
                    tbservice.Save(tbao);
                }
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, "Ký BBKS");

                IDeliverService deliver = new DeliverService();
                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IMailCanhBaoTCTService mailCanhBaoTCTService = IoC.Resolve<IMailCanhBaoTCTService>();
                try
                {
                    if (item.TongCongSuat >= 10000)
                    {
                        var listmail = mailCanhBaoTCTService.GetAll();
                        foreach (var mail in listmail)
                        {
                            var sendmailTCT = new SendMail();
                            sendmailTCT.EMAIL = mail.EMAIL;
                            sendmailTCT.TIEUDE = "Cảnh báo yêu cầu có công suất lớn hơn 10000kvA";
                            sendmailTCT.MA_DVIQLY = yeucau.MaDViQLy;
                            sendmailTCT.MA_YCAU_KNAI = yeucau.MaYeuCau;
                            Dictionary<string, string> bodyParamsTCT = new Dictionary<string, string>();
                            bodyParamsTCT.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                            bodyParamsTCT.Add("$donVi", yeucau.MaDViQLy);
                            bodyParamsTCT.Add("$duAnDien", yeucau.DuAnDien);
                            bodyParamsTCT.Add("$khuVuc", yeucau.DiaChiDungDien);
                            bodyParamsTCT.Add("$maYCau", yeucau.MaYeuCau);
                            bodyParamsTCT.Add("$ngayYCau", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsTCT.Add("$congsuat", item.TongCongSuat.ToString());
                            sendmailsrv.Process(sendmailTCT, "CanhBaoCS", bodyParamsTCT);
                        }
                        deliver.Deliver(item.MaYeuCau);

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                try
                {
                    IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
                    ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                    var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);

                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == nVienNhan).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = item.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = item.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", cvyc.TenKhachHang ?? cvyc.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", yeucau.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", yeucau.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", yeucau.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", cvyc.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", cvyc.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Điện lực ký BBKS");
                            sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                            IDeliverService deliverEmail = new DeliverService();
                            deliverEmail.Deliver(item.MaYeuCau);
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

        public bool Approve(BienBanKS item, KetQuaKS ketqua)
        {
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
                IThongBaoService thongBaoService = IoC.Resolve<IThongBaoService>();
                IKetQuaKSService kqservice = IoC.Resolve<IKetQuaKSService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IPhanCongKSService phanCongKSService = IoC.Resolve<IPhanCongKSService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                var cauhinhs = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, ketqua.MA_CVIEC);

                var cauhinh = cauhinhs.FirstOrDefault();
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                string maLoaiHSo = LoaiHSoCode.BB_KS;
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, maLoaiHSo);
                if (hoSo == null)
                {
                    hoSo = new HoSoGiayTo();
                    hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                    hoSo.TenHoSo = "Biên bản khảo sát lưới điện";
                }

                BeginTran();

                item.TrangThai = (int)TrangThaiBienBan.DaDuyet;
                yeucau.TrangThai = TrangThaiCongVan.BienBanKS;
                ketqua.TRANG_THAI = 1;

                var ttrinhtruoc = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, ketqua.MA_CVIEC_TRUOC, 0);
                if (ttrinhtruoc != null)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = ketqua.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

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

                tientrinh.NDUNG_XLY = ketqua.NDUNG_XLY;

                tientrinh.NGAY_BDAU = DateTime.Today;
                //if (ttrinhtruoc != null)
                //    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
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

                //Gửi thông báo đến người phân công khảo sát khi có trở ngại
                if (!ketqua.THUAN_LOI && cauhinh.TRANG_THAI_TIEP.HasValue)
                {
                    ketqua.TRANG_THAI = 0;
                    ketqua.THUAN_LOI = false;

                    item.ThuanLoi = false;
                    item.TrangThai = (int)TrangThaiBienBan.MoiTao;

                    yeucau.MaCViec = cauhinh.MA_CVIEC_TIEP;
                    yeucau.TrangThai = (TrangThaiCongVan)cauhinh.TRANG_THAI_TIEP.Value;
                    if (yeucau.TrangThai == TrangThaiCongVan.TiepNhan)
                    {
                        var thongbao = thongBaoService.GetbyYCau(item.MaDViQLy, item.MaYeuCau, LoaiThongBao.KhaoSat);
                        if (thongbao != null)
                        {
                            thongBaoService.Delete(thongbao);
                        }
                        var pc = phanCongKSService.GetbyMaYCau(yeucau.MaLoaiYeuCau, yeucau.MaYeuCau);
                        if (pc != null)
                        {
                            pc.TRANG_THAI = 0;
                            phanCongKSService.Save(pc);
                        }
                    }
                    if (yeucau.TrangThai == TrangThaiCongVan.PhanCongKS)
                    {
                        var pc = phanCongKSService.GetbyMaYCau(yeucau.MaLoaiYeuCau, yeucau.MaYeuCau);
                        if (pc != null)
                        {
                            pc.TRANG_THAI = 0;
                            phanCongKSService.Save(pc);
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
                    //if (ttrinhtruoc != null)
                    //    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                    ttrinhtngai.NGAY_KTHUC = DateTime.Now;

                    ttrinhtngai.NGAY_HEN = ketqua.NGAY_HEN;
                    ttrinhtngai.SO_LAN = 1;

                    ttrinhtngai.NGAY_TAO = DateTime.Now;
                    ttrinhtngai.NGAY_SUA = DateTime.Now;

                    ttrinhtngai.NGUOI_TAO = userdata.maNVien;
                    ttrinhtngai.NGUOI_SUA = userdata.maNVien;
                    ttrinhtngai.STT = nextstep + 1;
                    tientrinhsrv.Save(ttrinhtngai);
                }

                if (item.ThuanLoi)
                {
                    hoSo.TrangThai = 0;
                    hoSo.MaYeuCau = yeucau.MaYeuCau;
                    hoSo.MaDViQLy = yeucau.MaDViQLy;
                    hoSo.LoaiHoSo = maLoaiHSo;
                    hoSo.Data = item.Data;
                    hsogtosrv.Save(hoSo);
                }

                service.Save(yeucau);
                kqservice.Save(ketqua);
                Save(item);
                CommitTran();

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, "Duyệt BBKS");

                try
                {
                    IDeliverService deliver = new DeliverService();
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    if (item.ThuanLoi)
                    {
                        var sendmail = new SendMail();
                        sendmail.EMAIL = yeucau.Email;
                        sendmail.TIEUDE = "Kiểm tra, xác nhận biên bản khảo sát lưới điện";
                        sendmail.MA_DVIQLY = yeucau.MaDViQLy;
                        sendmail.MA_YCAU_KNAI = yeucau.MaYeuCau;
                        Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                        bodyParams.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                        bodyParams.Add("$maYCau", yeucau.MaYeuCau);
                        bodyParams.Add("$ngayYCau", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                        bodyParams.Add("$ngayKhaoSat", item.NgayKhaoSat.ToString("dd/MM/yyyy"));
                        sendmailsrv.Process(sendmail, "BienBanKhaoSat", bodyParams);
                    }
                    else
                    {
                        var ttrinhks = tientrinhsrv.GetbyYCau(yeucau.MaYeuCau, "PK", -1);
                        var nvPhanCong = userdatasrv.Query.Where(x => x.maNVien == ttrinhks.MA_NVIEN_NHAN).FirstOrDefault();
                        if (nvPhanCong != null)
                        {
                            if (!string.IsNullOrWhiteSpace(nvPhanCong.email))
                            {
                                var sendmailnv = new SendMail();
                                sendmailnv.EMAIL = nvPhanCong.email;
                                sendmailnv.TIEUDE = "Thông báo về khảo sát có trở ngại";
                                sendmailnv.MA_DVIQLY = item.MaDViQLy;
                                sendmailnv.MA_YCAU_KNAI = item.MaYeuCau;
                                Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                                bodyParamsNV.Add("$khachHang", yeucau.TenKhachHang ?? yeucau.NguoiYeuCau);
                                bodyParamsNV.Add("$maYCau", yeucau.MaYeuCau);
                                bodyParamsNV.Add("$donVi", yeucau.MaDViQLy);
                                bodyParamsNV.Add("$duAnDien", yeucau.DuAnDien);
                                bodyParamsNV.Add("$khuVuc", yeucau.DiaChiDungDien);
                                bodyParamsNV.Add("$ngaytiepnhan", yeucau.NgayYeuCau.ToString("dd/MM/yyyy"));
                                bodyParamsNV.Add("$nhanvien", nvPhanCong.fullName ?? nvPhanCong.username);
                                bodyParamsNV.Add("$lydo", item.TroNgai);
                                sendmailsrv.Process(sendmailnv, "CanhBaoNVTroNgai", bodyParamsNV);
                            }
                        }
                    }
                    deliver.Deliver(item.MaYeuCau);
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

        public BienBanKS GetbyYeuCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }

        public IList<BienBanKS> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
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

        public BienBanKS Update(BienBanKS bienban, IList<ThanhPhanKS> thanhPhans, out string message)
        {
            message = "";
            try
            {
                INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
                IThanhPhanKSService tpservice = IoC.Resolve<IThanhPhanKSService>();
                NoTransaction notran = new NoTransaction(notempsrv, NoType.BBKhaoSat);

                BeginTran();
                if (string.IsNullOrWhiteSpace(bienban.SoBienBan))
                {
                    lock (notran)
                    {
                        decimal no = notran.GetNextNo();
                        bienban.SoBienBan = notran.GetCode(bienban.MaDViQLy, no);
                    }
                }
                foreach (var tphan in bienban.ThanhPhans)
                {
                    tpservice.Delete(tphan);
                }

                bienban.ThanhPhans = thanhPhans;
                if (bienban.ThuanLoi)
                {
                    bienban.Data = bienban.GetPdf(true);
                }
                Save(bienban);

                foreach (var tphan in thanhPhans)
                {
                    tphan.BienBanID = bienban.ID;
                    tpservice.CreateNew(tphan);
                }

                notran.CommitTran();
                CommitTran();
                return bienban;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return null;
            }
        }

        public bool HuyHoSo(BienBanKS bienban, KetQuaKS ketqua)
        {
            try
            {
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();
                ICongVanYeuCauService ycausrv = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = ycausrv.GetbyMaYCau(ketqua.MA_YCAU_KNAI);
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiCongVan.PhanCongKS, 0);

                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(bienban.MaDViQLy, bienban.MaYeuCau, new int[] { 0, 2 });
                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);

                BeginTran();
                //Update lại trạng thái vế ghi nhận khảo sát
                yeucau.TrangThai = TrangThaiCongVan.Huy;
                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == tthaiycau.CVIEC_TRUOC && p.TRANG_THAI == 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = ketqua.MA_CVIEC_TRUOC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                    tientrinhs.Add(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_CVIEC = "HU";
                tientrinh.MA_CVIECTIEP = "HU";
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                tientrinh.MA_BPHAN_NHAN = ketqua.MA_BPHAN_GIAO;
                tientrinh.MA_NVIEN_NHAN = ketqua.MA_NVIEN_GIAO;
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

                ketqua.TRANG_THAI = 0;
                ketquasrv.Save(ketqua);
                if (bienban != null)
                {
                    bienban.TrangThai = (int)TrangThaiBienBan.MoiTao;
                    Save(bienban);
                }

                ycausrv.Save(yeucau);
   
                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var canhbao = new CanhBao();
                canhbao.LOAI_CANHBAO_ID = 11;
                canhbao.LOAI_SOLANGUI = 1;
                canhbao.MA_YC = ketqua.MA_YCAU_KNAI;
                canhbao.THOIGIANGUI = DateTime.Now;
                canhbao.TRANGTHAI_CANHBAO = 1;
                canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br> KH: " + yeucau.TenKhachHang + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + yeucau.NgayYeuCau + " ĐV: " + yeucau.MaDViQLy + " Khách hàng từ chối hoặc góp ý chỉnh sửa nội dung dự thảo thỏa thuận đấu nối, đơn vị hãy xử lý thông tin khách hàng từ chối ký thỏa thuận đấu nối trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
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
                deliver.PushTienTrinh(yeucau.MaDViQLy, yeucau.MaYeuCau);

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
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

                IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, $"Yêu cầu thực hiện khảo sát lưới điện, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {DateTime.Now.ToString("dd/MM/yyyy")}");
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Cancel(CongVanYeuCau yeucau, BienBanKS item, string noiDung)
        {
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
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
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng hủy yêu cầu: {yeucau.MaYeuCau}, ngày hủy: {DateTime.Now}";
                tbao.NguoiNhan = yeucau.NguoiLap;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = noiDung;
                tbservice.CreateNew(tbao);

                yeucau.TrangThai = TrangThaiCongVan.Huy;
                service.Save(yeucau);

                item.TrangThai = (int)TrangThaiBienBan.MoiTao;
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

                service.CommitTran();

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

        public bool Confirm(BienBanKS item, byte[] pdfdata)
        {
            try
            {
                string maLoaiHSo = LoaiHSoCode.BB_KS;
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                var service = IoC.Resolve<ICongVanYeuCauService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                IRepository repository = new FileStoreRepository();
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";

                BeginTran();
                yeucau.TrangThai = TrangThaiCongVan.BienBanKS;
                service.Save(yeucau);

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = item.MaYeuCau;
                tbao.MaDViQLy = item.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = item.MaCViec;
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng xác nhận biên bản khảo sát, ngày xác nhận: {DateTime.Now.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = item.NguoiLap;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = "Thực hiện ký biên bản khảo sát";
                tbservice.CreateNew(tbao);

                item.TroNgai = String.Empty;
                item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;
                item.Data = repository.Store(folder, pdfdata, item.Data);
                Save(item);

                if (hoso == null)
                {
                    hoso = new HoSoGiayTo();
                    hoso.MaHoSo = Guid.NewGuid().ToString("N");
                    hoso.TenHoSo = "Biên bản khảo sát lưới điện";
                    hoso.MaYeuCau = item.MaYeuCau;
                    hoso.MaDViQLy = item.MaDViQLy;
                    hoso.LoaiHoSo = maLoaiHSo;
                }
                hoso.Data = item.Data;
                hoso.TrangThai = 1;
                hsoservice.Save(hoso);
                CommitTran();

                try
                {
                    ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                    IKetQuaKSService ketQuaKSService = IoC.Resolve<IKetQuaKSService>();
                    ICongVanYeuCauService congVanYeuCauService = IoC.Resolve<ICongVanYeuCauService>();
                    var cvyc = congVanYeuCauService.GetbyMaYCau(item.MaYeuCau);
                    var kqks = ketQuaKSService.GetbyMaYCau(item.MaYeuCau);
                    if (kqks != null && cvyc != null)
                    {
                        var sendmailnv = new SendMail();
                        var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == kqks.MA_NVIEN_NHAN).FirstOrDefault();
                        if (nhanVienNhan != null)
                        {
                            if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                            {
                                sendmailnv.EMAIL = nhanVienNhan.email;
                                sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                                sendmailnv.MA_DVIQLY = item.MaDViQLy;
                                sendmailnv.MA_YCAU_KNAI = item.MaYeuCau;
                                Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                                bodyParamsNV.Add("$khachHang", cvyc.TenKhachHang ?? cvyc.NguoiYeuCau);
                                bodyParamsNV.Add("$donVi", yeucau.MaDViQLy);
                                bodyParamsNV.Add("$duAnDien", yeucau.DuAnDien);
                                bodyParamsNV.Add("$khuVuc", yeucau.DiaChiDungDien);
                                bodyParamsNV.Add("$maYCau", cvyc.MaYeuCau);
                                bodyParamsNV.Add("$ngaytiepnhan", cvyc.NgayYeuCau.ToString("dd/MM/yyyy"));
                                bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                                bodyParamsNV.Add("$buochientai", "Khách hàng ký BBKS");
                                sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);
                                IDeliverService deliver = new DeliverService();
                                deliver.Deliver(item.MaYeuCau);
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
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Yêu cầu khảo sát lại:
        /// - Cập nhật trạng thái của kết quả khảo sát về = 0
        /// - Cập nhật trạng thái của biên bản khảo sát về = 0
        /// - Cập nhật trạng thái của công văn yêu cầu đấu nối về trạng thái trước đó
        /// </summary>
        public bool KhaoSatLai(BienBanKS item, out string message)
        {
            message = "";
            try
            {
                string maLoaiHSo = LoaiHSoCode.BB_KS;
                IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();
                ICongVanYeuCauService ycausrv = IoC.Resolve<ICongVanYeuCauService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var yeucau = ycausrv.GetbyMaYCau(item.MaYeuCau);

                if (yeucau.TrangThai >= TrangThaiCongVan.DuThaoTTDN)
                {
                    message = "Không được hủy yêu cầu đã dự thảo đấu nối.";
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
                ketquaks.NDUNG_XLY = item.TroNgai;
                ketquasrv.Save(ketquaks);

                item.TrangThai = (int)TrangThaiBienBan.MoiTao;
                Save(item);

                yeucau.TrangThai = TrangThaiCongVan.GhiNhanKS;
                ycausrv.Save(yeucau);

                ThongBao tbao = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.KhaoSat);
                if (tbao == null) tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.KhaoSat;
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.NoiDung = $"Yêu cầu thực hiện khảo sát lại lưới điện, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {tbao.NgayHen.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = !string.IsNullOrWhiteSpace(tbao.NguoiNhan) ? tbao.NguoiNhan : ketquaks.MA_NVIEN_GIAO;
                tbao.BPhanNhan = ketquaks.MA_BPHAN_GIAO;
                tbao.CongViec = item.TroNgai;
                tbservice.Save(tbao);
                CommitTran();

                try
                {
                    IDataLoggingService dataLoggingService = IoC.Resolve<IDataLoggingService>();
                    dataLoggingService.CreateLog(yeucau.MaDViQLy, yeucau.MaYeuCau, userdata.maNVien, userdata.fullName, yeucau.MaCViec, $"Yêu cầu thực hiện khảo sát lại lưới điện, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {DateTime.Now.ToString("dd/MM/yyyy")}");
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
                message = ex.Message;
                return false;
            }
        }
    }
}