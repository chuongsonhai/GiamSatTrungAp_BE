using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/phancongkt")]
    public class PhanCongKTController : ApiController
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PhanCongKTController));

        [JwtAuthentication]
        [HttpPost]
        [Route("getdata")]
        public IHttpActionResult GetData(PhanCongTCRequest request)
        {
            try
            {
                IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var congvan = congvansrv.GetbyMaYCau(request.maYeuCau);
                var item = service.GetbyMaYCau(congvan.MaLoaiYeuCau, congvan.MaYeuCau, 0);

                if (item == null)
                {
                    var ttrinhtruoc = tientrinhsrv.GetbyYCau(congvan.MaYeuCau, congvan.MaCViec, -1);
                    item = new PhanCongTC();
                    item.MA_DVIQLY = congvan.MaDViQLy;

                    item.MA_YCAU_KNAI = congvan.MaYeuCau;
                    item.MA_DDO_DDIEN = congvan.MaDDoDDien;

                    item.MA_CVIEC = ttrinhtruoc.MA_CVIECTIEP;

                    item.NGAY_HEN = DateTime.Now;
                    item.NGAY_BDAU = ttrinhtruoc.NGAY_KTHUC.HasValue ? ttrinhtruoc.NGAY_KTHUC.Value : DateTime.Today;

                    item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                    item.MA_CVIEC_TRUOC = congvan.MaCViec;
                    item.LOAI = request.loai;
                }
                PhanCongTCModel model = new PhanCongTCModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] PhanCongTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
            IThongBaoService tbaosrv = IoC.Resolve<IThongBaoService>();

            var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
            var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
            model.LOAI = 0;
            if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                model.NDUNG_XLY = "Phân công kiểm tra điều kiện đóng điện điểm đấu nối";
            PhanCongTC item = new PhanCongTC();
            item = model.ToEntity(item);

            item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
            item.TRANG_THAI = 1;

            congvan.TrangThai = Core.TrangThaiNghiemThu.GhiNhanKT;

            ThongBao tbao = new ThongBao();
            tbao.MaYeuCau = congvan.MaYeuCau;
            tbao.MaDViQLy = congvan.MaDViQLy;
            tbao.NgayHen = DateTime.Now;
            tbao.MaCViec = congvan.MaCViec;
            tbao.TrangThai = TThaiThongBao.Moi;
            tbao.Loai = LoaiThongBao.KiemTra;
            tbao.DuAnDien = congvan.DuAnDien;
            tbao.KhachHang = congvan.NguoiYeuCau;
            string noidung = $"Kiểm tra điều kiện đóng điện điểm đấu nối, ngày hẹn: {item.NGAY_HEN.ToString("dd/MM/yyyy")}";

            tbao.NoiDung = noidung;
            tbao.NguoiNhan = item.MA_NVIEN_KS;
            tbao.BPhanNhan = model.MA_BPHAN_NHAN;
            tbao.CongViec = item.NDUNG_XLY;
            ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
            IDeliverService deliver = new DeliverService();
            IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
            try
            {
                var sendmailnv = new SendMail();
                var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == item.MA_NVIEN_KS).FirstOrDefault();
                if (nhanVienNhan != null)
                {
                    if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                    {
                        sendmailnv.EMAIL = nhanVienNhan.email;
                        sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                        sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                        sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                        Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                        bodyParamsNV.Add("$khachHang",congvan.NguoiYeuCau);
                        bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                        bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                        bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                        bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                        bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                        bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                        bodyParamsNV.Add("$buochientai", "Phân công kiểm tra");
                        sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);

                        deliver.Deliver(congvan.MaYeuCau);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                service.BeginTran();
                congvansrv.Save(congvan);
                service.CreateNew(item);

                tbaosrv.CreateNew(tbao);

                service.CommitTran();
                return Ok(new PhanCongTCModel(item));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                service.RolbackTran();
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] PhanCongTCModel model)
        {
            IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
            IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();
            IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
            IThongBaoService tbaosrv = IoC.Resolve<IThongBaoService>();
            try
            {
                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                var ttdn = ttdnservice.GetbyNo(congvan.SoThoaThuanDN, congvan.MaYeuCau);
                model.LOAI = 0;
                if (string.IsNullOrWhiteSpace(model.NDUNG_XLY))
                    model.NDUNG_XLY = "Phân công kiểm tra điều kiện đóng điện điểm đấu nối";
                var item = service.Getbykey(model.ID);
                item.MA_LOAI_YCAU = congvan.MaLoaiYeuCau;
                item = model.ToEntity(item);
                item.TRANG_THAI = 1;

                congvan.TrangThai = TrangThaiNghiemThu.GhiNhanKT;

                ThongBao tbao = new ThongBao();
                tbao.MaYeuCau = congvan.MaYeuCau;
                tbao.MaDViQLy = congvan.MaDViQLy;
                tbao.NgayHen = DateTime.Now;
                tbao.MaCViec = congvan.MaCViec;
                tbao.TrangThai = TThaiThongBao.Moi;
                tbao.Loai = LoaiThongBao.KiemTra;
                tbao.DuAnDien = congvan.DuAnDien;
                tbao.KhachHang = congvan.NguoiYeuCau;
                string noidung = $"Kiểm tra điều kiện đóng điện điểm đấu nối, ngày hẹn: {item.NGAY_HEN.ToString("dd/MM/yyyy")}";

                tbao.NoiDung = noidung;
                tbao.NguoiNhan = item.MA_NVIEN_KS;
                tbao.BPhanNhan = model.MA_BPHAN_NHAN;
                tbao.CongViec = item.NDUNG_XLY;

                service.BeginTran();
                congvansrv.Save(congvan);
                service.CreateNew(item);

                tbaosrv.CreateNew(tbao);

                service.CommitTran();

                ISendMailService sendmailsrv = IoC.Resolve<ISendMailService>();
                IDeliverService deliver = new DeliverService();
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                try
                {
                    var sendmailnv = new SendMail();
                    var nhanVienNhan = userdatasrv.Query.Where(x => x.maNVien == item.MA_NVIEN_KS).FirstOrDefault();
                    if (nhanVienNhan != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nhanVienNhan.email))
                        {
                            sendmailnv.EMAIL = nhanVienNhan.email;
                            sendmailnv.TIEUDE = "Thông báo về yêu cầu cần xử lý";
                            sendmailnv.MA_DVIQLY = congvan.MaDViQLy;
                            sendmailnv.MA_YCAU_KNAI = congvan.MaYeuCau;
                            Dictionary<string, string> bodyParamsNV = new Dictionary<string, string>();
                            bodyParamsNV.Add("$khachHang", congvan.NguoiYeuCau);
                            bodyParamsNV.Add("$donVi", congvan.MaDViQLy);
                            bodyParamsNV.Add("$duAnDien", congvan.DuAnDien);
                            bodyParamsNV.Add("$khuVuc", congvan.DiaChiDungDien);
                            bodyParamsNV.Add("$maYCau", congvan.MaYeuCau);
                            bodyParamsNV.Add("$ngaytiepnhan", congvan.NgayYeuCau.ToString("dd/MM/yyyy"));
                            bodyParamsNV.Add("$nhanvien", nhanVienNhan.fullName ?? nhanVienNhan.username);
                            bodyParamsNV.Add("$buochientai", "Phân công kiểm tra");
                            sendmailsrv.Process(sendmailnv, "CanhBaoNV", bodyParamsNV);

                            deliver.Deliver(congvan.MaYeuCau);
                        }

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return Ok(new PhanCongTCModel(item));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                service.RolbackTran();
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("cancel")]
        public IHttpActionResult Cancel([FromBody] PhanCongTCModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IPhanCongTCService service = IoC.Resolve<IPhanCongTCService>();

                var congvan = congvansrv.GetbyMaYCau(model.MA_YCAU_KNAI);
                PhanCongTC item = service.Getbykey(model.ID);
                if (congvan.TrangThai > TrangThaiNghiemThu.GhiNhanKT)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được phân công lại, yêu cầu đã ghi nhận kết quả kiểm tra";
                    result.success = false;
                    return Ok(result);
                }
                if (!service.CancelKiemTra(congvan, item))
                {
                    result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                    result.success = false;
                    return Ok(result);
                }
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                result.success = false;
                return Ok(result);
            }
        }
    }
}
