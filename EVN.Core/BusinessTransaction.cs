using EVN.Core.CMIS;
using EVN.Core.CMIS.Models;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVN.Core
{
    public class BusinessTransaction
    {
        private static ILog log = LogManager.GetLogger(typeof(BusinessTransaction));
        private static BusinessTransaction _instance;
        public static BusinessTransaction Instance
        {
            get
            {
                if (_instance == null) _instance = new BusinessTransaction();
                return _instance;
            }
        }

        /// <summary>
        /// Lấy thông tin công văn yêu cầu và các tài liệu đính kèm
        /// - Tạo ra 1 request theo thông tin công văn yêu cầu để gửi lên CMIS
        /// - Lấy mã yêu cầu từ CMIS, tạo công văn yêu cầu mới
        /// </summary>        
        public CongVanYeuCau RequestCMIS(string maYCau, out string message)
        {
            message = "";
            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            IMaDichVuService dvservice = IoC.Resolve<IMaDichVuService>();
            try
            {
                IHoSoGiayToService hsosrv = IoC.Resolve<IHoSoGiayToService>();
                var congvan = service.GetbyMaYCau(maYCau);
                if (congvan == null)
                {
                    message = $"Không tìm thấy yêu cầu tương ứng mã yêu cầu: {maYCau}";
                    return null;
                }
                IList<HoSoGiayTo> listhso = hsosrv.GetbyYeuCaudup2( congvan.MaYeuCau);
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();
                var cskhcfg = cfgservice.GetbyCode("EVN_CSKH");
                if (cskhcfg == null)
                {
                    message = "Chưa cấu hình url đăng ký mua điện trung áp";
                    return null;
                }
                string url = cskhcfg.Value;

                //Gửi yêu cầu lên CMIS
                var request = new YeuCauCmisRequest(congvan);
                string jsondata = JsonConvert.SerializeObject(request);
                string action = apiMuaDienTrungApNgoaiSinhHoat;
                var apicontent = ApiHelper.PostData(url, action, jsondata);
                log.ErrorFormat("url:{0}", url);
                log.ErrorFormat("action:{0}", action);
                log.ErrorFormat("jsondata:{0}", jsondata);

                if (apicontent == null) return null;
                var response = JsonConvert.DeserializeObject<YeuCauCmisResult>(apicontent);
                if (response.isError)
                {
                    message = response.message;
                    return null;
                }
                var data = response.data;
                string maYCauKNai = data["maYeuCauKhieuNai"].ToString();
                var yeucau = service.GetbyMaYCau(maYCauKNai);
                
                if (yeucau == null)
                {
                    message = $"Không tìm thấy yêu cầu tương ứng mã: {maYCauKNai}";
                    return null;
                }
                IList<HoSoGiayTo> dshoso = hsosrv.GetbyYeuCau(congvan.MaDViQLy, yeucau.MaYeuCau);
                service.BeginTran();
                yeucau.Data = congvan.Data;
                service.Save(yeucau);

                foreach (var hs in listhso)
                {
                    var hoSo = dshoso.FirstOrDefault(p => p.LoaiHoSo == hs.LoaiHoSo);
                    if (hoSo != null)
                    {
                        hoSo.Data = hs.Data;
                        hsosrv.Save(hoSo);
                        continue;
                    }
                    hoSo = new HoSoGiayTo();
                    hoSo.MaYeuCau = maYCauKNai;
                    hoSo.LoaiHoSo = hs.LoaiHoSo;
                    hoSo.TenHoSo = hs.TenHoSo;
                    hoSo.MaDViQLy = hs.MaDViQLy;
                    hoSo.TrangThai = hs.TrangThai;

                    hoSo.LoaiFile = hs.LoaiFile;
                    hoSo.Data = hs.Data;
                    hsosrv.CreateNew(hoSo);
                }
                var madvu = dvservice.Getbykey(maYCauKNai);
                if (madvu == null)
                {
                    madvu = new MaDichVu();
                    madvu.MA_YCAU_KNAI = maYCauKNai;
                    madvu.MA_DVIQLY = congvan.MaDViQLy;
                    madvu.NOI_DUNG_YCAU = congvan.NoiDungYeuCau;
                    madvu.TEN_KHANG = congvan.TenKhachHang;
                    madvu.EMAIL = congvan.Email;
                    madvu.DTHOAI = congvan.DienThoai;
                    madvu.ID_WEB = CommonUtils.RandomValue(8);
                    dvservice.CreateNew(madvu);
                }
                service.CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();
                var sendmail = new SendMail();
                sendmail.EMAIL = congvan.Email;
                sendmail.TIEUDE = "Tiếp nhận yêu cầu đề nghị đấu nối vào lưới điện trung áp";
                sendmail.MA_DVIQLY = congvan.MaDViQLy;
                sendmail.MA_YCAU_KNAI = yeucau.MaYeuCau;
                Dictionary<string, string> bodyParams = new Dictionary<string, string>();
                bodyParams.Add("$khachHang", congvan.TenKhachHang ?? congvan.NguoiYeuCau);
                bodyParams.Add("$maYCau", yeucau.MaYeuCau);
                bodyParams.Add("$maDichVu", madvu.ID_WEB);
                bodyParams.Add("$ngayYCau", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                bodyParams.Add("$ngayDuyet", congvan.NgayDuyet.ToString("dd/MM/yyyy"));
                sendmailsrv.Process(sendmail, "YeuCauDauNoi", bodyParams);
                deliver.Deliver(congvan.MaYeuCau);
                return yeucau;
            }
            catch (Exception ex)
            {
                service.RolbackTran();
                log.Error(ex);
                message = ex.Message;
                return null;
            }
        }

        public bool Duplicate(CongVanYeuCau congvan, string maYCauCu, out string message)
        {
            message = "";
            ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
            try
            {
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IPhanCongKSService pcksatsrv = IoC.Resolve<IPhanCongKSService>();
                IKetQuaKSService kqksatsrv = IoC.Resolve<IKetQuaKSService>();
                IBienBanKSService bbksatsrv = IoC.Resolve<IBienBanKSService>();
                IBienBanDNService bbdnsrv = IoC.Resolve<IBienBanDNService>();

                PhanCongKS pcks = pcksatsrv.GetbyMaYCau(congvan.MaLoaiYeuCau, maYCauCu);
                KetQuaKS kqks = kqksatsrv.GetbyMaYCau(maYCauCu);
                BienBanKS bbks = bbksatsrv.GetbyYeuCau(maYCauCu);
                BienBanDN bbdn = bbdnsrv.GetbyMaYeuCau(maYCauCu);

                var yeucau = service.GetbyMaYCau(maYCauCu);

                var tientrinhs = tientrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == maYCauCu).OrderBy(p => p.STT).ToList();
                var ttrinhtn = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == "TN");
                ttrinhtn.MA_YCAU_KNAI = congvan.MaYeuCau;
                ttrinhtn.NGAY_TAO = DateTime.Now;
                ttrinhtn.NGAY_SUA = DateTime.Now;

                ICmisProcessService cmisProcess = new CmisProcessService();
                var tiepnhan = cmisProcess.TiepNhanYeuCau(congvan, ttrinhtn);
                log.Error($"Tiep nhan CMIS: {tiepnhan}");
                if (!tiepnhan)
                    return false;

                congvan = service.SyncData(congvan.MaYeuCau);
                service.BeginTran();
                congvan.MaCViec = yeucau.MaCViec;
                congvan.TrangThai = yeucau.TrangThai;
                service.Save(congvan);

                if (pcks != null)
                {
                    var phancongks = new PhanCongKS();
                    phancongks.MA_DVIQLY = congvan.MaDViQLy;
                    phancongks.MA_YCAU_KNAI = congvan.MaYeuCau;
                    phancongks.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    phancongks.NDUNG_XLY = pcks.NDUNG_XLY;
                    phancongks.MA_BPHAN_GIAO = pcks.MA_BPHAN_GIAO;
                    phancongks.MA_NVIEN_GIAO = pcks.MA_NVIEN_GIAO;
                    phancongks.MA_BPHAN_NHAN = pcks.MA_BPHAN_NHAN;
                    phancongks.MA_NVIEN_NHAN = pcks.MA_NVIEN_NHAN;
                    phancongks.MA_CVIEC_TRUOC = pcks.MA_CVIEC_TRUOC;
                    phancongks.MA_CVIEC = pcks.MA_CVIEC;
                    phancongks.MA_LOAI_YCAU = pcks.MA_LOAI_YCAU;
                    phancongks.TRANG_THAI = pcks.TRANG_THAI;

                    phancongks.MA_NVIEN_KS = pcks.MA_NVIEN_KS;
                    phancongks.TEN_NVIEN_KS = pcks.TEN_NVIEN_KS;
                    phancongks.DTHOAI_NVIEN_KS = pcks.DTHOAI_NVIEN_KS;
                    phancongks.EMAIL_NVIEN_KS = pcks.EMAIL_NVIEN_KS;
                    pcksatsrv.CreateNew(phancongks);
                }

                if (kqks != null)
                {
                    var ketquaks = new KetQuaKS();
                    ketquaks.MA_DVIQLY = congvan.MaDViQLy;
                    ketquaks.MA_YCAU_KNAI = congvan.MaYeuCau;
                    ketquaks.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    ketquaks.KQUA_ID_BUOC = kqks.KQUA_ID_BUOC;
                    ketquaks.NDUNG_XLY = kqks.NDUNG_XLY;
                    ketquaks.MA_TNGAI = kqks.MA_TNGAI;
                    ketquaks.MA_BPHAN_GIAO = kqks.MA_BPHAN_GIAO;
                    ketquaks.MA_NVIEN_GIAO = kqks.MA_NVIEN_GIAO;
                    ketquaks.MA_BPHAN_NHAN = kqks.MA_BPHAN_NHAN;
                    ketquaks.MA_NVIEN_NHAN = kqks.MA_NVIEN_NHAN;
                    ketquaks.MA_CVIEC_TRUOC = kqks.MA_CVIEC_TRUOC;
                    ketquaks.MA_CVIEC = kqks.MA_CVIEC;
                    ketquaks.MA_LOAI_YCAU = kqks.MA_LOAI_YCAU;
                    ketquaks.NGUYEN_NHAN = kqks.NGUYEN_NHAN;
                    ketquaks.TRANG_THAI = kqks.TRANG_THAI;
                    ketquaks.THUAN_LOI = kqks.THUAN_LOI;
                    kqksatsrv.CreateNew(ketquaks);
                }

                if (bbks != null)
                {
                    var bienbanks = new BienBanKS();
                    bienbanks.MaDViQLy = congvan.MaDViQLy;
                    bienbanks.MaYeuCau = congvan.MaYeuCau;

                    bienbanks.MaDViTNhan = bbks.MaDViTNhan;
                    bienbanks.SoCongVan = bbks.SoCongVan;
                    bienbanks.NgayCongVan = bbks.NgayCongVan;
                    bienbanks.MaKH = bbks.MaKH;
                    bienbanks.SoBienBan = bbks.SoBienBan;
                    bienbanks.TenCongTrinh = bbks.TenCongTrinh;
                    bienbanks.DiaDiemXayDung = bbks.DiaDiemXayDung;
                    bienbanks.KHTen = bbks.KHTen;
                    bienbanks.KHDienThoai = bbks.KHDienThoai;
                    bienbanks.KHDaiDien = bbks.KHDaiDien;
                    bienbanks.KHChucDanh = bbks.KHChucDanh;

                    bienbanks.EVNDonVi = bbks.EVNDonVi;
                    bienbanks.EVNDaiDien = bbks.EVNDaiDien;
                    bienbanks.EVNChucDanh = bbks.EVNChucDanh;
                    bienbanks.NgayKhaoSat = bbks.NgayKhaoSat;
                    bienbanks.CapDienAp = bbks.CapDienAp;
                    bienbanks.TenLoDuongDay = bbks.TenLoDuongDay;
                    bienbanks.DiemDauDuKien = bbks.DiemDauDuKien;
                    bienbanks.DayDan = bbks.DayDan;
                    bienbanks.SoTramBienAp = bbks.SoTramBienAp;
                    bienbanks.SoMayBienAp = bbks.SoMayBienAp;
                    bienbanks.TongCongSuat = bbks.TongCongSuat;
                    bienbanks.NguoiLap = bbks.NguoiLap;
                    bienbanks.TrangThai = bbks.TrangThai;

                    bienbanks.Data = bbks.Data;
                    bienbanks.MaCViec = bbks.MaCViec;
                    bienbanks.ThuanLoi = bbks.ThuanLoi;
                    bienbanks.MaTroNgai = bbks.MaTroNgai;
                    bienbanks.TroNgai = bbks.TroNgai;
                    bbksatsrv.CreateNew(bienbanks);
                }

                if (bbdn != null)
                {
                    var bienbandn = new BienBanDN();
                    bienbandn.MaDViQLy = congvan.MaDViQLy;
                    bienbandn.MaYeuCau = congvan.MaYeuCau;
                    bienbandn.MaDDoDDien = bbdn.MaDDoDDien;

                    bienbandn.MaDViTNhan = bbdn.MaDViTNhan;
                    bienbandn.SoCongVan = bbdn.SoCongVan;
                    bienbandn.NgayCongVan = bbdn.NgayCongVan;
                    bienbandn.MaKH = bbdn.MaKH;
                    bienbandn.SoBienBan = bbdn.SoBienBan;
                    bienbandn.TenCongTrinh = bbdn.TenCongTrinh;
                    bienbandn.DiaDiemXayDung = bbdn.DiaDiemXayDung;
                    bienbandn.KHTen = bbdn.KHTen;
                    bienbandn.KHDienThoai = bbdn.KHDienThoai;
                    bienbandn.KHDaiDien = bbdn.KHDaiDien;
                    bienbandn.KHChucDanh = bbdn.KHChucDanh;

                    bienbandn.EVNDonVi = bbdn.EVNDonVi;
                    bienbandn.EVNDaiDien = bbdn.EVNDaiDien;
                    bienbandn.NgayKhaoSat = bbdn.NgayKhaoSat;
                    bienbandn.NguoiLap = bbdn.NguoiLap;
                    bienbandn.TrangThai = bbdn.TrangThai;

                    bienbandn.Data = bbdn.Data;
                    bienbandn.DocPath = bbdn.DocPath;

                    bienbandn.KHXacNhan = bbdn.KHXacNhan;
                    bienbandn.MaCViec = bbdn.MaCViec;
                    bienbandn.TroNgai = bbdn.TroNgai;
                    bbdnsrv.CreateNew(bienbandn);
                }

                int stt = 1;
                
                foreach (var ttrinh in tientrinhs)
                {
                    if (ttrinh.MA_CVIEC == "TVB") break;
                    if (ttrinh.MA_CVIEC == "TN")  
                        continue;
                    var tientrinh = new DvTienTrinh();                                        
                    tientrinh.MA_DVIQLY = congvan.MaDViQLy;
                    tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;

                    tientrinh.MA_BPHAN_GIAO = ttrinh.MA_BPHAN_GIAO;
                    tientrinh.MA_NVIEN_GIAO = ttrinh.MA_NVIEN_GIAO;

                    tientrinh.MA_BPHAN_NHAN = ttrinh.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinh.MA_NVIEN_NHAN;

                    tientrinh.MA_CVIEC = ttrinh.MA_CVIEC;
                    tientrinh.MA_CVIECTIEP = ttrinh.MA_CVIECTIEP;

                    tientrinh.MA_DDO_DDIEN =
                      int.TryParse(congvan.MaDDoDDien?.Trim(), out int maDdo)
                          ? (maDdo + 1).ToString()
                          : "1";



                    tientrinh.NDUNG_XLY = ttrinh.NDUNG_XLY;

                    tientrinh.NGAY_BDAU = ttrinh.NGAY_BDAU;

                    tientrinh.NGAY_KTHUC = ttrinh.NGAY_KTHUC;

                    tientrinh.NGAY_HEN = ttrinh.NGAY_HEN;
                    tientrinh.SO_LAN = 1;

                    tientrinh.NGAY_TAO = DateTime.Now;
                    tientrinh.NGAY_SUA = DateTime.Now;

                    tientrinh.NGUOI_TAO = ttrinh.NGUOI_TAO;
                    tientrinh.NGUOI_SUA = ttrinh.NGUOI_SUA;
                    tientrinh.STT = stt;
                    if (tientrinh.MA_CVIEC == "TN")
                        tientrinh.TRANG_THAI = 1;

                    tientrinhsrv.CreateNew(tientrinh);
                    stt++;
                }
                service.CommitTran();
                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);
                return true;
            }
            catch (Exception ex)
            {
                service.RolbackTran();
                log.Error(ex);
                message = ex.Message;
                return false;
            }
        }

        private string apiMuaDienTrungApNgoaiSinhHoat = "/api/DvhtAuthorized/DangKyMuaDienTrungApNgoaiSinhHoat";
    }
}