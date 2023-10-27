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
    public class BienBanTTService : FX.Data.BaseService<BienBanTT, int>, IBienBanTTService
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanTTService));
        public BienBanTTService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }

        public bool Cancel(BienBanTT item)
        {
            try
            {
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var yeucau = cvservice.GetbyMaYCau(item.MA_YCAU_KNAI);

                var cauhinh = cauhinhsrv.GetbyMaCViec(yeucau.MaLoaiYeuCau, "HU").FirstOrDefault();

                long nextstep = tientrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(yeucau.MaDViQLy, yeucau.MaYeuCau, new int[] { 0, 2 });

                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == yeucau.MaCViec && p.TRANG_THAI == 0);
                BeginTran();
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = cauhinh.MA_CVIEC_TRUOC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = item.MA_DVIQLY;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MA_DVIQLY;
                tientrinh.MA_NVIEN_NHAN = userdata.maNVien;

                if (ttrinhtruoc != null)
                {
                    tientrinh.MA_BPHAN_GIAO = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_GIAO = ttrinhtruoc.MA_NVIEN_NHAN;

                    tientrinh.MA_BPHAN_NHAN = ttrinhtruoc.MA_BPHAN_NHAN;
                    tientrinh.MA_NVIEN_NHAN = ttrinhtruoc.MA_NVIEN_NHAN;
                }

                tientrinh.MA_CVIEC = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                tientrinh.NDUNG_XLY = "Hủy yêu cầu kiểm tra điều kiện đóng điện điểm đấu nối và nghiệm thu";

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;
                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Today;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                tientrinhsrv.CreateNew(tientrinh);

                yeucau.TrangThai = TrangThaiNghiemThu.Huy;

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.CanhBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Khách hàng hủy yêu cầu: {yeucau.MaYeuCau}, ngày hủy: {DateTime.Now}";
                tbao.NguoiNhan = userdata.maNVien;
                tbao.BPhanNhan = tientrinh.MA_BPHAN_NHAN;
                tbao.CongViec = yeucau.LyDoHuy;
                service.CreateNew(tbao);

                item.TRANG_THAI = 0;

                Save(item);
                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(yeucau.MaDViQLy, yeucau.MaYeuCau);

                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool Approve(YCauNghiemThu congvan, BienBanTT item, KetQuaTC ketqua)
        {
            try
            {
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICauHinhDKService cauhinhsrv = IoC.Resolve<ICauHinhDKService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();
                IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                string maCViec = congvan.MaCViec;
                var tthaiycau = ttycausrv.GetbyStatus((int)TrangThaiNghiemThu.NghiemThu, 1);
                if (tthaiycau != null)
                    maCViec = tthaiycau.CVIEC_TRUOC;

                item.TRANG_THAI = (int)TrangThaiBienBan.DaDuyet;
                congvan.TrangThai = TrangThaiNghiemThu.NghiemThu;

                var cauhinh = cauhinhsrv.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec).FirstOrDefault();
                IList<DvTienTrinh> tientrinhs = tientrinhsrv.ListNew(congvan.MaDViQLy, congvan.MaYeuCau, new int[] { 0, 2 });
                long nextstep = tientrinhsrv.LastbyMaYCau(congvan.MaYeuCau);
                if (nextstep == 0) nextstep = cauhinh.ORDERNUMBER;

                BeginTran();
                var ttrinhtruoc = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == ketqua.MA_CVIEC_TRUOC && p.TRANG_THAI == 0);
                if (ttrinhtruoc != null && ttrinhtruoc.TRANG_THAI != 1)
                {
                    ttrinhtruoc.NGAY_KTHUC = DateTime.Today;
                    ttrinhtruoc.MA_CVIECTIEP = ketqua.MA_CVIEC;
                    ttrinhtruoc.TRANG_THAI = 2;
                    tientrinhsrv.Save(ttrinhtruoc);
                }

                DvTienTrinh tientrinh = tientrinhs.FirstOrDefault(p => p.MA_CVIEC == maCViec && p.TRANG_THAI == 0);
                if (tientrinh == null) tientrinh = new DvTienTrinh();
                tientrinh.MA_BPHAN_GIAO = item.MA_DVIQLY;
                tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                tientrinh.MA_BPHAN_NHAN = item.MA_DVIQLY;
                tientrinh.MA_NVIEN_NHAN = userdata.maNVien;

                tientrinh.MA_CVIEC = maCViec;
                tientrinh.MA_CVIECTIEP = cauhinh.MA_CVIEC_TIEP;
                tientrinh.MA_DDO_DDIEN = congvan.MaDDoDDien;
                tientrinh.MA_DVIQLY = congvan.MaDViQLy;

                tientrinh.MA_YCAU_KNAI = congvan.MaYeuCau;
                tientrinh.NDUNG_XLY = "Tạo biên bản treo tháo lên CMIS";

                tientrinh.NGAY_BDAU = DateTime.Today;
                if (ttrinhtruoc != null)
                    tientrinh.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.Value;

                tientrinh.NGAY_KTHUC = DateTime.Now;

                tientrinh.NGAY_HEN = DateTime.Today;
                tientrinh.SO_LAN = 1;

                tientrinh.NGAY_TAO = DateTime.Now;
                tientrinh.NGAY_SUA = DateTime.Now;

                tientrinh.NGUOI_TAO = userdata.maNVien;
                tientrinh.NGUOI_SUA = userdata.maNVien;
                if (tientrinh.STT == 0)
                    tientrinh.STT = nextstep;

                if (item.TRANG_THAI == (int)TrangThaiBienBan.DaDuyet)
                {
                    tientrinh.TRANG_THAI = 1;
                    ICmisProcessService cmisProcess = new CmisProcessService();
                    if (!cmisProcess.LapBBanTrTh(item, tientrinh))
                        throw new Exception("Chưa tạo được biên bản treo tháo trên CMIS");

                    string maLoaiHSo = LoaiHSoCode.BB_TT;
                    HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(congvan.MaDViQLy, congvan.MaYeuCau, maLoaiHSo);
                    if (hoSo == null)
                    {
                        hoSo = new HoSoGiayTo();
                        hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                        hoSo.TenHoSo = "Biên bản treo tháo";
                    }
                    hoSo.TrangThai = 0;
                    hoSo.MaYeuCau = congvan.MaYeuCau;
                    hoSo.MaDViQLy = congvan.MaDViQLy;
                    hoSo.LoaiHoSo = maLoaiHSo;
                    hoSo.Data = item.Data;
                    hsogtosrv.Save(hoSo);
                }

                ThongBao tbao = tbservice.GetbyYCau(item.MA_DVIQLY, item.MA_YCAU_KNAI, LoaiThongBao.TreoThao);
                if (tbao != null && (tbao.TrangThai == TThaiThongBao.Moi || tbao.TrangThai == TThaiThongBao.QuaHan))
                {
                    tbao.TrangThai = TThaiThongBao.DaXuLy;
                    tbservice.Save(tbao);
                }

                ketqua.TRANG_THAI = 1;
                kqservice.Save(ketqua);

                tientrinhsrv.Save(tientrinh);
                Save(item);

                congvan.TrangThai = TrangThaiNghiemThu.NghiemThu;
                service.Update(congvan);
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

        public IList<BienBanTT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
        {
            var query = Query.Where(p => p.NGAY_TAO >= fromdate.Date && p.NGAY_TAO < todate.Date.AddDays(1));
            if (status > -1)
                query = query.Where(p => p.TRANG_THAI == status);
            if (!string.IsNullOrWhiteSpace(maDViQly))
                query = query.Where(p => p.MA_DVIQLY == maDViQly);
            if (!string.IsNullOrWhiteSpace(maYCau))
                query = query.Where(p => p.MA_YCAU_KNAI == maYCau);
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.SO_BB.Contains(keyword) || p.TEN_KHACHHANG.Contains(keyword) || p.VTRI_LDAT.Contains(keyword));
            total = query.Count();
            query = query.OrderByDescending(p => p.NGAY_TAO);
            return query.Skip(pageindex * pagesize).Take(pagesize).ToList();
        }

        public BienBanTT GetbyMaYCau(string maYCau)
        {
            return Get(p => p.MA_YCAU_KNAI == maYCau);
        }

        public BienBanTT CreateNew(BienBanTT bienban, IList<CongTo> congTos, IList<MayBienDong> mayBienDongs, IList<MayBienDienAp> mayBienDienAps)
        {
            INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
            ICongToService congToService = IoC.Resolve<ICongToService>();
            IMayBienDongService mayBienDongService = IoC.Resolve<IMayBienDongService>();
            IMayBienDienApService mayBienDienApService = IoC.Resolve<IMayBienDienApService>();
            NoTransaction notran = new NoTransaction(notempsrv, NoType.BBKTDongDien);
            try
            {
                BeginTran();
                lock (notran)
                {
                    decimal no = notran.GetNextNo();
                    bienban.SO_BB = notran.GetCode(bienban.MA_DVIQLY, no);
                }
                CreateNew(bienban);
                foreach (var item in congTos)
                {
                    item.BBAN_ID = bienban.ID;
                    congToService.CreateNew(item);
                }
                foreach (var item in mayBienDongs)
                {
                    item.BBAN_ID = bienban.ID;
                    mayBienDongService.CreateNew(item);
                }
                foreach (var item in mayBienDienAps)
                {
                    item.BBAN_ID = bienban.ID;
                    mayBienDienApService.CreateNew(item);
                }
                notran.CommitTran();
                CommitTran();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return null;
            }
            return bienban;
        }

        public BienBanTT Update(BienBanTT bienban, IList<CongTo> congTos, IList<MayBienDong> mayBienDongs, IList<MayBienDienAp> mayBienDienAps)
        {
            try
            {
                INoTemplateService notempsrv = IoC.Resolve<INoTemplateService>();
                ICongToService congToService = IoC.Resolve<ICongToService>();
                IMayBienDongService mayBienDongService = IoC.Resolve<IMayBienDongService>();
                IMayBienDienApService mayBienDienApService = IoC.Resolve<IMayBienDienApService>();
                NoTransaction notran = new NoTransaction(notempsrv, NoType.BBKTDongDien);

                BeginTran();
                if (string.IsNullOrWhiteSpace(bienban.SO_BB))
                {
                    lock (notran)
                    {
                        decimal no = notran.GetNextNo();
                        bienban.SO_BB = notran.GetCode(bienban.MA_DVIQLY, no);
                    }
                }
                Save(bienban);
                foreach (var item in bienban.CongTos)
                {
                    congToService.Delete(item);
                }
                foreach (var item in congTos)
                {
                    item.BBAN_ID = bienban.ID;
                    congToService.CreateNew(item);
                }

                foreach (var item in bienban.MayBienDongs)
                {
                    mayBienDongService.Delete(item);
                }
                foreach (var item in mayBienDongs)
                {
                    item.BBAN_ID = bienban.ID;
                    mayBienDongService.CreateNew(item);
                }

                foreach (var item in bienban.MayBienDienAps)
                {
                    mayBienDienApService.Delete(item);
                }
                foreach (var item in mayBienDienAps)
                {
                    item.BBAN_ID = bienban.ID;
                    mayBienDienApService.CreateNew(item);
                }

                notran.CommitTran();
                CommitTran();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                RolbackTran();
                return null;
            }

            IKetQuaTCService kqservice = IoC.Resolve<IKetQuaTCService>();
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            var bienbantt = Getbykey(bienban.ID);
            var yeucau = congvansrv.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
            var ketqua = kqservice.GetbyMaYCau(bienbantt.MA_YCAU_KNAI);
            if (Approve(yeucau, bienbantt, ketqua))
                bienban.TRANG_THAI = bienbantt.TRANG_THAI;
            return bienban;
        }

        public bool UpdatebyCMIS(BienBanTT item, YCauNghiemThu yeucau, byte[] pdfdata)
        {
            try
            {
                string maLoaiHSo = LoaiHSoCode.BB_TT;

                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IRepository repository = new FileStoreRepository();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var hoso = hsoservice.Get(p => p.MaDViQLy == yeucau.MaDViQLy && p.MaYeuCau == yeucau.MaYeuCau && p.LoaiHoSo == maLoaiHSo);
                if (hoso == null)
                {
                    hoso = new HoSoGiayTo();
                    hoso.MaHoSo = Guid.NewGuid().ToString("N");
                    hoso.TenHoSo = "Biên bản treo tháo";
                    hoso.LoaiHoSo = maLoaiHSo;
                }

                yeucau.TrangThai = TrangThaiNghiemThu.KetQuaTC;
                hoso.TrangThai = 0;

                int totalSign = PdfSignUtil.NamesSigned(pdfdata);
                var cusSign = PdfSignUtil.HasSigned(pdfdata, "BÊN MUA ĐIỆN"); 
                if (cusSign == 0)
                    cusSign = PdfSignUtil.NamesSigned(pdfdata);
                log.Error($"cusSign: {cusSign}");

                if (cusSign == 1 && item.TRANG_THAI < (int)TrangThaiBienBan.KhachHangKy)
                {
                    item.TRANG_THAI = (int)TrangThaiBienBan.KhachHangKy;
                    hoso.TrangThai = 1;
                    yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                }
                if (totalSign >= 2)
                {
                    if (hoso.TrangThai < 2 && item.KyNVNP && item.KyNVTT)
                        hoso.TrangThai = 2;
                    if (yeucau.TrangThai < TrangThaiNghiemThu.NghiemThu)
                        yeucau.TrangThai = TrangThaiNghiemThu.NghiemThu;
                }

                string folder = $"{item.MA_DVIQLY}/{item.MA_YCAU_KNAI}";
                BeginTran();
                if (yeucau.TrangThai == TrangThaiNghiemThu.NghiemThu)
                {
                    ThongBao tbaott = tbservice.GetbyYCau(item.MA_DVIQLY, item.MA_YCAU_KNAI, LoaiThongBao.TreoThao);
                    ThongBao tbao = tbservice.GetbyYCau(item.MA_DVIQLY, item.MA_YCAU_KNAI, LoaiThongBao.NghiemThu);
                    if (tbao == null)
                    {
                        tbao = new ThongBao();
                        tbao.MaYeuCau = yeucau.MaYeuCau;
                        tbao.MaDViQLy = yeucau.MaDViQLy;
                        tbao.NgayHen = DateTime.Now;
                        tbao.MaCViec = yeucau.MaCViec;
                        tbao.TrangThai = TThaiThongBao.Moi;
                        tbao.Loai = LoaiThongBao.NghiemThu;
                        tbao.NguoiNhan = tbaott != null ? tbaott.NguoiNhan : item.NGUOI_TAO;
                        tbao.BPhanNhan = tbaott != null ? tbaott.BPhanNhan : userdata.maBPhan;
                        tbao.NgayHen = DateTime.Now;
                        tbao.NoiDung = $"Nghiệm thu, mã yêu cầu: {yeucau.MaYeuCau}";
                        tbao.DuAnDien = yeucau.DuAnDien;
                        tbao.KhachHang = yeucau.NguoiYeuCau;
                        tbservice.Save(tbao);
                    }
                }
                service.Save(yeucau);
                item.Data = repository.Store(folder, pdfdata, item.Data);
                hoso.Data = item.Data;
                hoso.MaYeuCau = yeucau.MaYeuCau;
                hoso.MaDViQLy = yeucau.MaDViQLy;
                hsoservice.Save(hoso);
                Save(item);
                CommitTran();

                ttrinhsrv.DongBoTienDo(yeucau);
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                return false;
            }
        }

        public bool HuyKetQua(BienBanTT bienban, KetQuaTC ketqua)
        {
            try
            {
                IYCauNghiemThuService ycausrv = IoC.Resolve<IYCauNghiemThuService>();
                IKetQuaTCService ketquasrv = IoC.Resolve<IKetQuaTCService>();
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();

                var yeucau = ycausrv.GetbyMaYCau(bienban.MA_YCAU_KNAI);
                BeginTran();
                ketqua.TRANG_THAI = 0;
                ketquasrv.Save(ketqua);

                if (bienban != null)
                {
                    bienban.TRANG_THAI = (int)TrangThaiBienBan.MoiTao;
                    Save(bienban);
                }
                yeucau.TrangThai = TrangThaiNghiemThu.PhanCongTC;
                ycausrv.Save(yeucau);

                ThongBao tbao = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.TreoThao);
                if (tbao == null) tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.TreoThao;
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.NoiDung = $"Yêu cầu thực hiện treo tháo, mã yêu cầu: {yeucau.MaYeuCau}, ngày hẹn: {tbao.NgayHen.ToString("dd/MM/yyyy")}";
                tbao.NguoiNhan = !string.IsNullOrWhiteSpace(tbao.NguoiNhan) ? tbao.NguoiNhan : ketqua.MA_NVIEN_GIAO;
                tbao.BPhanNhan = ketqua.MA_BPHAN_GIAO;
                tbao.CongViec = ketqua.NGUYEN_NHAN;
                tbservice.Save(tbao);

                ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                var lcanhbao = CBservice.Query.Where(p => p.TRANGTHAI_CANHBAO <= 6);
                var lcanhbao1 = lcanhbao.FirstOrDefault(p => p.LOAI_CANHBAO_ID == 13 && p.MA_YC == bienban.MA_YCAU_KNAI);
                var canhbao = new CanhBao();
                if (lcanhbao1 == null)
                {

                    canhbao.LOAI_CANHBAO_ID = 13;
                    canhbao.LOAI_SOLANGUI = 1;
                    canhbao.MA_YC = yeucau.MaYeuCau;
                    canhbao.THOIGIANGUI = DateTime.Now;
                    canhbao.TRANGTHAI_CANHBAO = 1;
                    canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                    canhbao.NOIDUNG = "Loại cảnh báo 13 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + tbao.KhachHang + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + yeucau.NgayLap + " ĐV: " + yeucau.MaDViQLy + "<br> Ngành điện gặp trở ngại trong quá trình treo tháo thiết bị đo đếm với lý do Khách hàng hủy yêu cầu: " + yeucau.MaYeuCau + ", ngày hủy:" + DateTime.Now.ToString("dd/MM/yyyy") + " , đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và khắc phục theo đúng qui định.";
                }
                else
                {
                    var checkTonTai1 = CBservice.CheckExits11(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                    var check_tontai_mycau1 = CBservice.GetByMaYeuCautontai(lcanhbao1.MA_YC, lcanhbao1.LOAI_CANHBAO_ID);
                    if (checkTonTai1)
                    {
                        canhbao.LOAI_CANHBAO_ID = 13;
                        canhbao.LOAI_SOLANGUI = check_tontai_mycau1.LOAI_SOLANGUI + 1;
                        canhbao.MA_YC = yeucau.MaYeuCau;
                        canhbao.THOIGIANGUI = DateTime.Now;
                        canhbao.TRANGTHAI_CANHBAO = 1;
                        canhbao.DONVI_DIENLUC = yeucau.MaDViQLy;
                        canhbao.NOIDUNG = "Loại cảnh báo 13 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + tbao.KhachHang + ", SĐT: " + yeucau.DienThoai + ", ĐC: " + yeucau.DiaChiCoQuan + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận:" + yeucau.NgayLap + " ĐV: " + yeucau.MaDViQLy + "<br> Ngành điện gặp trở ngại trong quá trình treo tháo thiết bị đo đếm với lý do Khách hàng hủy yêu cầu: " + yeucau.MaYeuCau + ", ngày hủy:" + DateTime.Now.ToString("dd/MM/yyyy") + " , đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và khắc phục theo đúng qui định.";
                    }
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