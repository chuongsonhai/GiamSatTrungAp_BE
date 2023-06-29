using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVN.Core.Implements
{
    public class BienBanNTService : FX.Data.BaseService<BienBanNT, int>, IBienBanNTService
    {
        ILog log = LogManager.GetLogger(typeof(BienBanNTService));
        public BienBanNTService(string sessionFactoryConfigPath, string connectionString = null) : base(sessionFactoryConfigPath, connectionString)
        {
        }
        public BienBanNT GetbyMaYeuCau(string mayeucau)
        {
            return Get(x => x.MaYeuCau == mayeucau);
        }
        public IList<BienBanNT> GetbyFilter(string maDViQly, string maYCau, string keyword, int status, DateTime fromdate, DateTime todate, int pageindex, int pagesize, out int total)
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

        public bool Approve(BienBanNT item, out string message)
        {
            message = "";
            try
            {
                IThongBaoService tbservice = IoC.Resolve<IThongBaoService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();                
                IRepository repository = new FileStoreRepository();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IHopDongService hdservice = IoC.Resolve<IHopDongService>();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var hopdong = hdservice.GetbyMaYCau(item.MaYeuCau);
                if (hopdong == null || hopdong.TrangThai < (int)TrangThaiBienBan.HoanThanh)
                {
                    message = "Hợp đồng chưa đủ chữ ký, vui lòng hoàn thành hợp đồng.";
                    return false;
                }

                var yeucau = service.GetbyMaYCau(item.MaYeuCau);
                ttrinhsrv.DongBoTienDo(yeucau);
                if (!ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "KHD"))
                {
                    message = "Khách hàng chưa ký hợp đồng";
                    return false;
                }
                
                var pdfdata = repository.GetData(item.Data);
                if (pdfdata == null)
                {
                    message = "Không tìm thấy file biên bản nghiệm thu";
                    return false;
                }
                string maLoaiHSo = LoaiHSoCode.BB_NT;
                ICmisProcessService cmisdeliver = new CmisProcessService();
                if (!cmisdeliver.UploadPdf(item.MaDViQLy, item.MaYeuCau, pdfdata, maLoaiHSo))
                {
                    log.Error($"Lỗi upload file lên CMIS: {item.MaYeuCau} - {maLoaiHSo}");
                    message = "Chưa đồng bộ được file lên CMIS";
                    return false;
                }
                long nextstep = ttrinhsrv.LastbyMaYCau(yeucau.MaYeuCau);
                
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                HoSoGiayTo hoSo = hsogtosrv.GetHoSoGiayTo(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                if (hoSo == null)
                {
                    hoSo = new HoSoGiayTo();
                    hoSo.MaHoSo = Guid.NewGuid().ToString("N");
                    hoSo.TenHoSo = "Biên bản nghiệm thu, đóng điện";
                }
                hoSo.TrangThai = 2;
                hoSo.MaYeuCau = item.MaYeuCau;
                hoSo.MaDViQLy = item.MaDViQLy;
                hoSo.LoaiHoSo = maLoaiHSo;
                hoSo.Data = item.Data;
               

                BeginTran();
                if (!ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "DHD"))
                {
                    DvTienTrinh ttrinhhdh = new DvTienTrinh();
                    ttrinhhdh.MA_BPHAN_GIAO = userdata.maBPhan;
                    ttrinhhdh.MA_NVIEN_GIAO = userdata.maNVien;

                    ttrinhhdh.MA_BPHAN_NHAN = userdata.maBPhan;
                    ttrinhhdh.MA_NVIEN_NHAN = userdata.maNVien;

                    ttrinhhdh.MA_CVIEC = "DHD";
                    ttrinhhdh.MA_CVIECTIEP = "HT";
                    ttrinhhdh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                    ttrinhhdh.MA_DVIQLY = yeucau.MaDViQLy;

                    ttrinhhdh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                    ttrinhhdh.NDUNG_XLY = "Hoàn thành";

                    ttrinhhdh.NGAY_BDAU = DateTime.Today;
                    ttrinhhdh.NGAY_KTHUC = DateTime.Now;

                    ttrinhhdh.NGAY_HEN = DateTime.Today;
                    ttrinhhdh.SO_LAN = 1;

                    ttrinhhdh.NGAY_TAO = DateTime.Now;
                    ttrinhhdh.NGAY_SUA = DateTime.Now;

                    ttrinhhdh.NGUOI_TAO = userdata.maNVien;
                    ttrinhhdh.NGUOI_SUA = userdata.maNVien;
                    ttrinhhdh.STT = nextstep + 1;

                    ttrinhsrv.CreateNew(ttrinhhdh);
                }

                if (!ttrinhsrv.Query.Any(p => p.MA_YCAU_KNAI == item.MaYeuCau && p.MA_CVIEC == "HT"))
                {
                    DvTienTrinh tientrinh = new DvTienTrinh();
                    tientrinh.MA_BPHAN_GIAO = userdata.maBPhan;
                    tientrinh.MA_NVIEN_GIAO = userdata.maNVien;

                    tientrinh.MA_BPHAN_NHAN = userdata.maBPhan;
                    tientrinh.MA_NVIEN_NHAN = userdata.maNVien;

                    tientrinh.MA_CVIEC = "HT";
                    tientrinh.MA_CVIECTIEP = "KT";
                    tientrinh.MA_DDO_DDIEN = yeucau.MaDDoDDien;
                    tientrinh.MA_DVIQLY = yeucau.MaDViQLy;

                    tientrinh.MA_YCAU_KNAI = yeucau.MaYeuCau;
                    tientrinh.NDUNG_XLY = "Hoàn thành";

                    tientrinh.NGAY_BDAU = DateTime.Today;
                    tientrinh.NGAY_KTHUC = DateTime.Now;

                    tientrinh.NGAY_HEN = DateTime.Today;
                    tientrinh.SO_LAN = 1;

                    tientrinh.NGAY_TAO = DateTime.Now;
                    tientrinh.NGAY_SUA = DateTime.Now;

                    tientrinh.NGUOI_TAO = userdata.maNVien;
                    tientrinh.NGUOI_SUA = userdata.maNVien;
                    if (tientrinh.STT == 0)
                        tientrinh.STT = nextstep;
                    ttrinhsrv.CreateNew(tientrinh);
                }

                hsogtosrv.Save(hoSo);

                yeucau.TrangThai = TrangThaiNghiemThu.HoanThanh;
                yeucau.MaCViec = "HT";
                service.Save(yeucau);

                item.TrangThai = (int)TrangThaiBienBan.HoanThanh;
                Save(item);
                ThongBao tbaont = tbservice.GetbyYCau(yeucau.MaDViQLy, yeucau.MaYeuCau, LoaiThongBao.NghiemThu);
                if (tbaont != null && (tbaont.TrangThai == TThaiThongBao.Moi || tbaont.TrangThai == TThaiThongBao.QuaHan))
                {
                    tbaont.TrangThai = TThaiThongBao.DaXuLy;
                    tbservice.Save(tbaont);
                }

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = yeucau.MaYeuCau;
                tbao.MaDViQLy = yeucau.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = yeucau.MaCViec;
                tbao.Loai = LoaiThongBao.ThongBao;
                tbao.TrangThai = TThaiThongBao.ThongBao;
                tbao.NoiDung = $"Nghiệm thu, mã yêu cầu: {yeucau.MaYeuCau}, ngày hoàn thành: {DateTime.Now.ToString("dd/MM/yyyy")}";

                tbao.NguoiNhan = tbaont != null? tbaont.NguoiNhan : item.NguoiLap;
                tbao.BPhanNhan = userdata.maBPhan;
                tbao.CongViec = yeucau.LyDoHuy;
                tbservice.CreateNew(tbao);
                CommitTran();

                IDeliverService deliver = new DeliverService();
                deliver.PushTienTrinh(yeucau.MaDViQLy, yeucau.MaYeuCau);
                return true;
            }
            catch (Exception ex)
            {
                RolbackTran();
                log.Error(ex);
                message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return false;
            }
        }
    }
}
