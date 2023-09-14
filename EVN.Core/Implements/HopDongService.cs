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
    public class HopDongService : FX.Data.BaseService<HopDong, int>, IHopDongService
    {
        private ILog log = LogManager.GetLogger(typeof(HopDongService));
        public HopDongService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool Cancel(HopDong item)
        {
            try
            {
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var congvan = cvservice.GetbyMaYCau(item.MaYeuCau);

                var cauhinh = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, "HU").FirstOrDefault();

                var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, congvan.MaCViec, 0);
                BeginTran();
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = cauhinh.MA_CVIEC_TRUOC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = item.MaDViQLy;
                //tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MaDViQLy;
                //tientrinh.MA_NVIEN_NHAN = userdata.maNVien;

                if (ttrinhtruoc != null)
                {
                    tientrinh.MA_BPHAN_GIAO = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_GIAO = ttrinhtruoc.MA_NVIEN_NHAN;

                    tientrinh.MA_BPHAN_NHAN = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinhtruoc.MA_NVIEN_NHAN;
                }

                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = "Hủy yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Today;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                //tientrinh.NGUOI_TAO = userdata.maNVien;
                //tientrinh.NGUOI_SUA = userdata.maNVien;
                tientrinhsrv.CreateNew(tientrinh);

                congvan.TrangThai = TrangThaiNghiemThu.Huy;
                item.TrangThai = 0;

                Save(item);

                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var canhbao = new CanhBao();
                canhbao.LOAI_CANHBAO_ID = 16;
                canhbao.LOAI_SOLANGUI = 1;
                canhbao.MA_YC = congvan.MaYeuCau;
                canhbao.THOIGIANGUI = DateTime.Now;
                canhbao.TRANGTHAI_CANHBAO = 1;
                canhbao.DONVI_DIENLUC = congvan.MaDViQLy;
                canhbao.NOIDUNG = "Cảnh báo lần…(" + canhbao.LOAI_SOLANGUI + ") <br>KH " + item.KHTen + ", SĐT " + item.DienThoai + ", ĐC: " + item.DiaChi + ", MaYC " + canhbao.MA_YC + ", ngày tiếp nhận:" + congvan.NgayLap + "ĐV: " +item.MaDViQLy + " Thời gian ký thỏa thuận đấu nối vượt quá 02 năm, đơn vị hãy xử lý thông tin trên hệ thống Ứng dụng cấp điện mới trực tuyến và giám sát các chỉ số tiếp cận điện năng.";
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
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Notify(HopDong item, string maCViec, string deptId, string staffCode, DateTime ngayHen, string noiDung, out string message)
        {
            message = "";
            try
            {
                IThoaThuanDNChiTietService historysrv = IoC.Resolve<IThoaThuanDNChiTietService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var congvan = service.GetbyMaYCau(item.MaYeuCau);

                var cauhinhs = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);
                var cauhinh = cauhinhs.FirstOrDefault();

                ThoaThuanDNChiTiet history = historysrv.GetbyType(item.MaYeuCau, DoingBusinessType.KhaoSat);

                BeginTran();
                var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, congvan.MaCViec, 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = maCViec;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                item.TrangThai = 1;
                Save(item);

                congvan.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                congvan.MaCViec = maCViec;
                service.Save(congvan);

                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = deptId;
                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;
                tientrinh.MA_NVIEN_NHAN = staffCode;
                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = noiDung;

                tientrinh.NGAY_BDAU = DateTime.Today;
                //if (ttrinhtruoc != null)
                //    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.HasValue ? ttrinhtruoc.NGAY_KTHUC.Value : DateTime.Now;
                tientrinh.NGAY_KTHUC = DateTime.Now;
                tientrinh.NGAY_HEN = ngayHen;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                tientrinhsrv.CreateNew(tientrinh);

                if (history == null)
                {
                    history = new ThoaThuanDNChiTiet();
                    history.MaYeuCau = item.MaYeuCau;
                    history.NgayYeuCau = DateTime.Now;
                    history.DuAnDien = congvan.DuAnDien;
                    history.DiaChiDungDien = congvan.DiaChiDungDien;
                }
                history.TrangThai = 2;
                history.MoTa = "Gửi thư mời nghiệm thu, đóng điện: " + userdata.maNVien;
                history.NgayThucHien = DateTime.Now;
                history.UpdateDate = DateTime.Now;
                historysrv.Save(history);
                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(congvan.MaDViQLy, congvan.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return false;
            }
        }

        public HopDong GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MaYeuCau == maYCau);
        }

        public IList<HopDong> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.NgayHopDong >= fromdate.Date && p.NgayHopDong < todate.Date.AddDays(1));
            if (status > -1)
                query = query.Where(p => p.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(maDViQly))
                query = query.Where(p => p.MaDViQLy == maDViQly);
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MaYeuCau == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.KHMa.Contains(keyword) || p.KHTen.Contains(keyword));
            total = query.Count();
            query = query.OrderByDescending(p => p.NgayHopDong);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public bool UpdatebyCMIS(HopDong item, byte[] pdfdata)
        {
            try
            {
                string maLoaiHSo = LoaiHSoCode.HD_NSH;

                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IRepository repository = new FileStoreRepository();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                ttrinhsrv.DongBoTienDo(yeucau);

                var hoso = hsoservice.Get(p => p.MaDViQLy == item.MaDViQLy && p.MaYeuCau == item.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                if (hoso == null)
                {
                    hoso = new HoSoGiayTo();
                    hoso.MaHoSo = Guid.NewGuid().ToString("N");
                    hoso.TenHoSo = "Dự thảo hợp đồng mua bán điện";
                    hoso.LoaiHoSo = maLoaiHSo;
                }
                hoso.MaYeuCau = item.MaYeuCau;
                hoso.MaDViQLy = item.MaDViQLy;
                hoso.Data = item.Data;

                item.TrangThai = (int)TrangThaiBienBan.DaDuyet;
                checkTienTrinh(item, yeucau, hoso);

                if (item.TrangThai < (int)TrangThaiBienBan.KhachHangKy)
                    checkSignature(item, yeucau, hoso, pdfdata);

                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}/HDNSH";
                item.Data = repository.Store(folder, pdfdata, item.Data);
                hoso.Data = item.Data;
                BeginTran();
                hsoservice.Save(hoso);

                service.Save(yeucau);
                Save(item);
                CommitTran();
                //if (item.TrangThai == (int)TrangThaiBienBan.HoanThanh || yeucau.TrangThai == TrangThaiNghiemThu.NghiemThu)
                //{
                //    ttrinhsrv.ThemTTrinhNT((int)TrangThaiNghiemThu.EVNKyHD, yeucau, userdata);
                //    ttrinhsrv.ThemTTrinhNT((int)TrangThaiNghiemThu.HoanThanh, yeucau, userdata);
                //}
                if (yeucau.TrangThai == TrangThaiNghiemThu.PhanCongTC)
                {
                    ttrinhsrv.ThemTTrinhNT((int)TrangThaiNghiemThu.PhanCongTC, yeucau, userdata);
                    ttrinhsrv.ThemTTrinhNT((int)TrangThaiNghiemThu.KetQuaTC, yeucau, userdata);
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

        void checkTienTrinh(HopDong item, YCauNghiemThu yeucau, HoSoGiayTo hoso)
        {
            IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
            var ttrinhDHD = ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "DHD");
            var ttrinhKHD = ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "KHD");
            if (ttrinhDHD && ttrinhKHD)
            {
                hoso.TrangThai = 2;
                if (yeucau.TrangThai < TrangThaiNghiemThu.PhanCongTC)
                    yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                else
                {
                    if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                        yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                }
                if (item.TrangThai < (int)TrangThaiBienBan.HoanThanh)
                    item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
            }
            else
            {
                if (ttrinhKHD)
                {
                    hoso.TrangThai = 1;
                    if (item.TrangThai < (int)TrangThaiBienBan.KhachHangKy)
                        item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;
                    if (yeucau.TrangThai < TrangThaiNghiemThu.PhanCongTC)
                        yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                    else
                    {
                        if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                            yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                    }
                }
                if (ttrinhDHD)
                {
                    if (yeucau.TrangThai < TrangThaiNghiemThu.PhanCongTC)
                        yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                    else
                    {
                        if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                            yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                    }
                }
            }
        }

        void checkSignature(HopDong item, YCauNghiemThu yeucau, HoSoGiayTo hoso, byte[] pdfdata)
        {
            int totalSign = PdfSignUtil.NamesSigned(pdfdata);

            bool cusSigned = PdfSignUtil.CustomerSigned(pdfdata) > 0;

            if (totalSign == 2)
            {
                hoso.TrangThai = 2;
                if (yeucau.TrangThai < TrangThaiNghiemThu.PhanCongTC)
                    yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                else
                {
                    if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                        yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                }
                if (item.TrangThai < (int)TrangThaiBienBan.HoanThanh)
                    item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
            }
            else
            {
                if (cusSigned)
                {
                    if (item.TrangThai < (int)TrangThaiBienBan.KhachHangKy)
                    {
                        item.TrangThai = (int)TrangThaiBienBan.KhachHangKy;
                        hoso.TrangThai = 1;
                        if (totalSign == 1)
                        {
                            item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
                            hoso.TrangThai = 2;
                        }                        
                    }
                    else
                    {
                        item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
                        hoso.TrangThai = 2;
                    }
                    
                    if (yeucau.TrangThai < TrangThaiNghiemThu.PhanCongTC)
                        yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                    else
                    {
                        if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                            yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                    }                    
                }
            }
        }
    }
}
