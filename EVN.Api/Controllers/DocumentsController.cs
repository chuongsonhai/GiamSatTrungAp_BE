using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Utils;
using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [JwtAuthentication]
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(DocumentsController));

        [HttpGet]
        [Route("getinfo/{maYCau}/{token}")]
        public IHttpActionResult GetInfo(string maYCau, string token)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                log.Error($"{maYCau} - {token}");
                IMaDichVuService service = IoC.Resolve<IMaDichVuService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService bienBanKSService= IoC.Resolve<IBienBanKSService>();

                var congvan = cvservice.GetbyMaYCau(maYCau);
                if (congvan == null)
                {
                    log.Error($"Chưa có tiến trình tương ứng: {maYCau}");
                    result.message = "Yêu cầu chưa được tiếp nhận hoặc mã yêu cầu không đúng";
                    result.success = false;
                    return Ok(result);
                }

                var maDVu = service.Getbykey(maYCau);
                if (maDVu == null)
                {
                    log.Error($"Chưa có mã dịch vụ tương ứng: {maYCau}");
                    result.message = "Mã yêu cầu hoặc mã xác nhận không hợp lệ";
                    result.success = false;
                    return Ok(result);
                }
                if (maDVu.ID_WEB != token)
                {
                    log.Error($"Chưa có mã ID_WEB: {maYCau}");
                    result.message = "Mã yêu cầu hoặc mã xác nhận không hợp lệ";
                    result.success = false;
                    return Ok(result);
                }
                string maDViQly = maDVu.MA_DVIQLY;
                string maLoaiHSo = LoaiHSoCode.HD_NSH;
                var hsoHDong = hsoservice.GetHoSoGiayTo(maDViQly, maYCau, maLoaiHSo);
                if (hsoHDong != null)
                {
                    IHopDongService hdongsrv = IoC.Resolve<IHopDongService>();
                    var hdong = hdongsrv.GetbyMaYCau(maDVu.MA_YCAU_KNAI);
                    if (hdong != null && hsoHDong.TrangThai < 2)
                    {
                        ICmisProcessService cmisProcess = new CmisProcessService();
                        byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                        if (pdfdata != null && pdfdata.Length > 0)
                        {
                            hdongsrv.UpdatebyCMIS(hdong, pdfdata);
                            hsoHDong = hsoservice.GetHoSoGiayTo(maDViQly, maYCau, maLoaiHSo);
                        }
                    }
                }

                maLoaiHSo = LoaiHSoCode.BB_TT;
                var hsoTThao = hsoservice.GetHoSoGiayTo(maDViQly, maYCau, maLoaiHSo);
                if (hsoTThao != null)
                {
                    IBienBanTTService bbtthaosrv = IoC.Resolve<IBienBanTTService>();
                    var bienBanTT = bbtthaosrv.GetbyMaYCau(maYCau);
                    if (bienBanTT != null && hsoTThao.TrangThai < 2)
                    {
                        IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                        ICmisProcessService cmisProcess = new CmisProcessService();
                        var yeucau = congvansrv.GetbyMaYCau(maYCau);
                        byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                        if (pdfdata != null && pdfdata.Length > 0)
                        {
                            bbtthaosrv.UpdatebyCMIS(bienBanTT, yeucau, pdfdata);
                            hsoTThao = hsoservice.GetHoSoGiayTo(maDViQly, maYCau, maLoaiHSo);
                        }
                    }
                }

                var list = hsoservice.GetbyYeuCau(maDVu.MA_DVIQLY, maDVu.MA_YCAU_KNAI);
                IList<DocumentData> data = new List<DocumentData>();
                foreach (var item in list)
                {
                    data.Add(new DocumentData(item));
                }
                IList<DvTienTrinh> tientrinhs = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == maYCau).OrderBy(p => p.STT).ToList();
                IList<TTrinhYCau> cviecs = new List<TTrinhYCau>();
                foreach (var item in tientrinhs)
                {
                    cviecs.Add(new TTrinhYCau(item));
                }
                YeuCauResult ycau = new YeuCauResult(congvan);
                ycau.tenNgDaiDien = congvan.TenKhachHangUQ;
                ycau.dienThoaiNgDaiDien = congvan.SoDienThoaiKHUQ;
                ycau.idWeb = token;
                ycau.danhSachHoSo = data;
                ycau.danhSachCViec = cviecs;
                if (tientrinhs.Any(p => p.MA_CVIEC == "KT" || p.MA_CVIEC == "HT"))
                    ycau.trangThai = "HT";
               
                result.data = ycau;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = "Mã yêu cầu hoặc mã xác nhận không hợp lệ";
                result.success = false;
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("getpdf/{maYCau}/{loaiHSo}")]
        public IHttpActionResult GetPdf(string maYCau, string loaiHSo)
        {
            log.Error($"{maYCau} - {loaiHSo}");
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();
                var yeucau = cvservice.GetbyMaYCau(maYCau);
                if (yeucau == null)
                {
                    result.message = "Mã yêu cầu không đúng";
                    result.success = false;
                    return Ok(result);
                }
                var hoSo = service.GetHoSoGiayTo(yeucau.MaDViQLy, yeucau.MaYeuCau, loaiHSo);
                if (hoSo == null)
                {
                    log.Error($"Không tìm thấy hồ sơ, giấy tờ: {yeucau.MaDViQLy} - {yeucau.MaYeuCau} - {loaiHSo}");
                    throw new Exception("Hồ sơ không có trên hệ thống hoặc đã bị xóa");
                }

                IRepository repository = new FileStoreRepository();
                if (string.IsNullOrWhiteSpace(hoSo.Data))
                {
                    log.Error($"Không có path của file: {yeucau.MaDViQLy} - {yeucau.MaYeuCau} - {loaiHSo}");
                    throw new Exception("Hồ sơ không có trên hệ thống hoặc đã bị xóa");
                }

                var pdfdata = repository.GetData(hoSo.Data);
                if (pdfdata == null)
                {
                    log.Error($"Không tìm thấy file: {hoSo.Data}");
                    throw new Exception("Hồ sơ không có trên hệ thống hoặc đã bị xóa");
                }

                FileDataResult data = new FileDataResult();
                data.Code = maYCau;
                data.Base64Data = Convert.ToBase64String(pdfdata);
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = ex.Message;
                result.success = false;
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("updatepdf")]
        public IHttpActionResult UpdatePdf([FromBody] PdfRequest request)
        {
           // MultipleResponseFileResult resultTotal = new MultipleResponseFileResult();
            ResponseFileResult result = new ResponseFileResult();
            //ResponseFileResult result1 = new ResponseFileResult();
            try
            {
                log.Error($"ThuanLoi: {request.ThuanLoi}, NoiDungXuLy: {request.NoiDungXuLy}");
                byte[] pdfdata = null;
                if (!string.IsNullOrWhiteSpace(request.Base64Data))
                    pdfdata = Convert.FromBase64String(request.Base64Data);

                result.success = DocumentUtils.UpdatePdf(request.maYCau, request.loaiHSo, pdfdata, request.NoiDungXuLy, request.ThuanLoi, request.Huy);
                //resultTotal.Result1 = result1;
                if(request.loaiHSo == "56" && request.Huy == true)
                {
                    IBienBanKSService service = IoC.Resolve<IBienBanKSService>();
                    IKetQuaKSService ketquasrv = IoC.Resolve<IKetQuaKSService>();

                    var bienbanks = service.GetbyYeuCau(request.maYCau);
                    var ketquaks = ketquasrv.GetbyMaYCau(request.maYCau);
                    ketquaks.NGUYEN_NHAN = ketquaks.NDUNG_XLY = request.NoiDungXuLy;
                    bienbanks.TroNgai = request.NoiDungXuLy;
                    result.success = service.HuyHoSo2(bienbanks, ketquaks);
                    //resultTotal.Result2 = result;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = ex.Message;
                result.success = false;
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("updatestatus")]
        public IHttpActionResult UpdateStatus(UpdateStatusRequest request)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                result.success = DocumentUtils.UpdateStatus(request.maYCau, request.loaiHoSo, request.noiDungXuLy, request.trangThai);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = ex.Message;
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("upload")]
        public IHttpActionResult Upload(DocumentRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                IYCauNghiemThuService cvservice = IoC.Resolve<IYCauNghiemThuService>();
                IRepository repository = new FileStoreRepository();

                var yeucau = cvservice.GetbyMaYCau(request.maYeuCau);
                if (yeucau == null)
                {
                    result.success = false;
                    result.message = "Mã yêu cầu không hợp lệ";
                    return Ok(result);
                }
                foreach (var item in request.Items)
                {
                    byte[] fileData = Convert.FromBase64String(item.Base64Data);
                    string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";

                    var hoSo = service.GetHoSoGiayTo(yeucau.MaDViQLy, request.maYeuCau, item.loaiHoSo);
                    if (hoSo == null) hoSo = new HoSoGiayTo();
                    hoSo.TrangThai = 1;
                    hoSo.MaYeuCau = yeucau.MaYeuCau;
                    hoSo.MaDViQLy = yeucau.MaDViQLy;
                    hoSo.LoaiHoSo = item.loaiHoSo;
                    hoSo.TenHoSo = item.tenHoSo;
                    hoSo.LoaiFile = item.loaiFile;
                    hoSo.Data = repository.Store(folder, fileData, hoSo.Data, hoSo.LoaiFile);
                    service.Save(hoSo);
                }
                service.CommitChanges();

                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }
    }

    public class PdfRequest
    {
        public string maDViQLy { get; set; }
        public string maYCau { get; set; }
        public string loaiHSo { get; set; }
        public bool ThuanLoi { get; set; } = true;
        public bool Huy { get; set; } = false;
        public string NoiDungXuLy { get; set; }
        public string Base64Data { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string loaiHoSo { get; set; }
        public string maYCau { get; set; }
        public int trangThai { get; set; } = 0; //0: Đã ký, 1: Cần sửa, 2: Hủy yêu cầu
        public string noiDungXuLy { get; set; }
    }

    public class DocumentData
    {
        public DocumentData(HoSoGiayTo entity)
        {
            maHoSo = entity.MaHoSo;
            maDViQly = entity.MaDViQLy;
            maHoSoGiayTo = entity.LoaiHoSo;
            tenHoSoGiayTo = entity.TenHoSo;
            maYeuCau = entity.MaYeuCau;
            tinhTrang = entity.TrangThai;
        }

        public string maHoSo { get; set; }
        public string maDViQly { get; set; }
        public string maHoSoGiayTo { get; set; }
        public string tenHoSoGiayTo { get; set; }
        public string maYeuCau { get; set; }
        public int tinhTrang { get; set; } = 0;
    }

    public class YeuCauResult
    {
        public YeuCauResult(CongVanYeuCau item)
        {
            maDonViQuanLy = item.MaDViQLy;
            maYeuCauKhieuNai = item.MaYeuCau;
            tenNguoiYeuCau = item.NguoiYeuCau;
            tenKhachHang = item.TenKhachHang;
            coQuanChuQuan = item.CoQuanChuQuan;
            diaChiDungDien = item.DiaChiDungDien;
            tenLoaiYeuCau = "Cấp mới trung áp";
            ngayYeuCau = item.NgayYeuCau.ToString("dd/MM/yyyy");
            chungMinhThu = item.MST;
            dienThoai = item.DienThoai;
            email = item.Email;
        }
        public string maDonViQuanLy { get; set; }
        public string maYeuCauKhieuNai { get; set; }
        public string tenNguoiYeuCau { get; set; }
        public string tenKhachHang { get; set; }
        public string coQuanChuQuan { get; set; }
        public string diaChiDungDien { get; set; }
        public string tenLoaiYeuCau { get; set; }
        public string ngayYeuCau { get; set; }
        public string chungMinhThu { get; set; }
        public string dienThoai { get; set; }
        public string tenNgDaiDien { get; set; }
        public string dienThoaiNgDaiDien { get; set; }
        public string email { get; set; }
        public string trangThai { get; set; } = "LPA";
        public string tienDo { get; set; }
        public decimal tienNo { get; set; } = 0;

        public string idWeb { get; set; }
        public IList<DocumentData> danhSachHoSo { get; set; } = new List<DocumentData>();
        public IList<TTrinhYCau> danhSachCViec { get; set; } = new List<TTrinhYCau>();
    }

    public class TTrinhYCau
    {
        public TTrinhYCau(DvTienTrinh ttrinh)
        {
            MaCViec = ttrinh.MA_CVIEC;
            NoiDung = ttrinh.NDUNG_XLY;
            NgayBDau = ttrinh.NGAY_BDAU.ToString("dd/MM/yyyy");
            TrangThai = ttrinh.TRANG_THAI;
            NgayKThuc = TrangThai > 0 && ttrinh.NGAY_KTHUC.HasValue ? ttrinh.NGAY_KTHUC.Value.ToString("dd/MM/yyyy") : "";
        }
        public string NgayBDau { get; set; }
        public string NgayKThuc { get; set; }
        public string MaCViec { get; set; }
        public string NoiDung { get; set; }
        public int TrangThai { get; set; }
    }
}
