using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
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
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/ycaudaunoi")]
    public class YCauDauNoiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(YCauDauNoiController));

        //[JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(YeuCauFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime synctime = DateTime.Today;
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();

                var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
                if (cskhcfg != null && cskhcfg.Value == "1")
                {
                    service.SyncHU();
                }

                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromDate = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    toDate = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                request.Filter.keyword = !string.IsNullOrWhiteSpace(request.Filter.keyword) ? request.Filter.keyword.Trim() : request.Filter.keyword;
                var list = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.keyword, request.Filter.khachhang, request.Filter.status, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<YeuCauDataRequest>();
                foreach (var item in list)
                {
                    var model = new YeuCauDataRequest(item);
                    var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);
                    if (bbks != null)
                    {
                        model.TroNgai = bbks.TroNgai;
                    }
                    listModel.Add(model);
                }
                result.total = total;
                result.data = listModel;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.data = new List<YeuCauDataRequest>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("LayDuLieuChart")]
        public IHttpActionResult LayDuLieuChart(YeuCauFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                int total = 0;
                DateTime synctime = DateTime.Today;
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                ICongViecService viecService = IoC.Resolve<ICongViecService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetList(request.maDViQLy, fromDate, toDate, out total);
                var groupByMaCV = list.GroupBy(x => x.MaCViec);
                var model = new ChartResult();
                foreach (var item in groupByMaCV)
                {
                    var first = item.ToList().FirstOrDefault();
                    var a = item.ToList();
                    string tenCV = viecService.Getbykey(first.MaCViec).MA_CVIEC;
                    model.series.Add(item.ToList().Count);
                    model.labels.Add(tenCV);
                }
                result.total = total;
                result.data = model;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("layyeucau")]
        public IHttpActionResult LayYeuCau(YeuCauCViecRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                IThongBaoService tbaosrv = IoC.Resolve<IThongBaoService>();
                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var fromDate = DateTime.Today;
                var toDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(request.tuNgay))
                    fromDate = DateTime.ParseExact(request.tuNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.denNgay))
                    toDate = DateTime.ParseExact(request.denNgay, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = tbaosrv.GetbyNVien(userdata.maDViQLy, userdata.maNVien, fromDate, toDate, 20);

                var data = new List<ThongBaoData>();
                foreach (var item in list)
                    data.Add(new ThongBaoData(item));
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không tìm thấy mã yêu cầu";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("getbykey")]
        public IHttpActionResult GetbyKey(TTYeuCauRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ICongViecService cviecsrv = IoC.Resolve<ICongViecService>();

                var yeucau = service.GetbyMaYCau(request.maYCau);
                if (yeucau == null || yeucau.TrangThai == TrangThaiCongVan.MoiTao || yeucau.TrangThai == TrangThaiCongVan.Huy)
                    throw new Exception();

                DvTienTrinh lastTTrinh = ttrinhsrv.Query.Where(p => p.MA_YCAU_KNAI == yeucau.MaYeuCau).OrderByDescending(p => p.NGUOI_TAO).FirstOrDefault();
                TTinYeuCauResponse data = new TTinYeuCauResponse(yeucau);
                data.CongViec = cviecsrv.Getbykey(lastTTrinh.MA_CVIEC).TEN_CVIEC;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Không tìm thấy mã yêu cầu";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                var item = service.Getbykey(id);                
                if (item.TrangThai >= TrangThaiCongVan.TiepNhan && string.IsNullOrWhiteSpace(item.MaDDoDDien))
                {
                    item = service.SyncData(item.MaYeuCau);
                }
                if (string.IsNullOrWhiteSpace(item.BenNhan) && !string.IsNullOrWhiteSpace(item.MaDViQLy))
                {
                    IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();
                    var org = orgservice.GetbyCode(item.MaDViQLy);
                    item.BenNhan = org.orgName;
                    service.Save(item);
                    service.CommitChanges();
                }
                YeuCauDauNoiData model = new YeuCauDauNoiData(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] YeuCauDataRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                var item = new CongVanYeuCau();
                log.Error(JsonConvert.SerializeObject(model));
                item = model.ToEntity(item);
                if (string.IsNullOrWhiteSpace(item.BenNhan) && !string.IsNullOrWhiteSpace(item.MaDViQLy))
                {
                    IOrganizationService orgservice = IoC.Resolve<IOrganizationService>();
                    var org = orgservice.GetbyCode(item.MaDViQLy);
                    if (org != null)
                        item.BenNhan = org.orgName;
                }
                string message = "";

                item.MaHinhThuc = "WEB EVNHANOI";
                if (!service.CreateNew(item, out message))
                {
                    log.Error(message);
                    result.success = false;
                    result.message = message;
                    return Ok(result);
                }

                IRepository repository = new FileStoreRepository();
                var pdfdata = repository.GetData(item.Data);
                result.success = true;

                FileDataResult data = new FileDataResult();
                data.Code = item.Fkey;
                data.Base64Data = Convert.ToBase64String(pdfdata);
                result.data = data;
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

        [JwtAuthentication]
        [HttpPost]
        [Route("updatePDF")]
        public IHttpActionResult UpdatePDF([FromBody] YeuCauSignRequest model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();
                var item = service.Get(p => p.Fkey == model.Code);
                if (item == null)
                {
                    log.Error("Không có CongVanYeuCau: " + model.Code);
                    result.success = false;
                    result.message = "Không tìm thấy pdf tương ứng";
                    return Ok(result);
                }

                var hoso = hsogtosrv.GetbyCode(model.Code);
                if (hoso == null)
                {
                    log.Error("Không có HoSoGiayTo: " + model.Code);
                    result.success = false;
                    result.message = "Không tìm thấy pdf tương ứng";
                    return Ok(result);
                }

                IRepository repository = new FileStoreRepository();
                var pdfdata = Convert.FromBase64String(model.Base64Data);

                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, pdfdata, item.Data);
                hoso.Data = item.Data;
                hoso.TrangThai = 1;
                hsogtosrv.Save(hoso);
                service.Save(item);
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

        [JwtAuthentication]
        [HttpPost]
        [Route("trahoso")]
        public IHttpActionResult TraHoSo(CancelModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                var item = service.GetbyMaYCau(model.maYCau);
                
                item.LyDoHuy = model.noiDung;
                result.success = service.Cancel(item);

                if (item.TrangThai == TrangThaiCongVan.Huy)
                {
                    ICanhBaoService CBservice = IoC.Resolve<ICanhBaoService>();
                    var canhbao = new CanhBao();
                        canhbao.LOAI_CANHBAO_ID = 9;
                        canhbao.LOAI_SOLANGUI = 1;
                        canhbao.MA_YC = item.MaYeuCau;
                        canhbao.THOIGIANGUI = DateTime.Now;
                        canhbao.TRANGTHAI_CANHBAO = 1;
                        canhbao.DONVI_DIENLUC = item.MaDViQLy;
                        canhbao.NOIDUNG = "Loại cảnh báo 9 - lần " + canhbao.LOAI_SOLANGUI + " <br>KH: " + item.TenKhachHang + ", SĐT: " + item.DienThoai + ", ĐC: " + item.DiaChiDungDien + ", MaYC: " + canhbao.MA_YC + ", ngày tiếp nhận: " + item.NgayLap + " ĐV: " + item.MaDViQLy + "<br> Yêu cầu thỏa thuận đấu nối của khách hàng bị từ chối tiếp nhận với lý do " + model.noiDung + ", đơn vị kiểm tra lý do cập nhật trên hệ thống với thực tế tại hồ sơ và tính chất trở ngại (có thể khắc phục hoặc phải hủy yêu cầu cấp điện)";
      
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
                    result.success = true;
                    return Ok(result);
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }

        //[JwtAuthentication]
        [HttpPost]
        [Route("duyethoso")]
        public IHttpActionResult DuyetHoSo([FromBody] ApproveModel model)
        {
            try
            {
                string message = "";

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                var item = service.Getbykey(model.id);
                item = service.DongBo(item);
                var congviec = cvservice.Getbykey(model.maCViec);
                model.noiDung = congviec.TEN_CVIEC;
                log.ErrorFormat("duyethoso item :{0}", JsonConvert.SerializeObject(item));
                if (!service.DuyetHoSo(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung, out message))
                    return BadRequest();
                YeuCauDauNoiData result = new YeuCauDauNoiData(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("yeucaukhaosat")]
        public IHttpActionResult YeuCauKhaoSat([FromBody] ApproveModel model)
        {
            try
            {
                string message = "";

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                var congviec = cvservice.Getbykey(model.maCViec);
                model.noiDung = congviec.TEN_CVIEC;
                var item = service.Getbykey(model.id);

                if (!service.YeuCauKhaoSat(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung, out message))
                    return BadRequest();
                YeuCauDauNoiData result = new YeuCauDauNoiData(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("chuyentiep")]
        public IHttpActionResult ChuyenTiep([FromBody] ChuyenTiepRequest model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                result.success = service.ChuyenTiep(model.maYCau, model.maDViTNhan);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("duplicate")]
        public IHttpActionResult Duplicate([FromBody] TTYeuCauRequest model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string message = "";
                var yeucau = BusinessTransaction.Instance.RequestCMIS(model.maYCau, out message);
                if (yeucau == null)
                {
                    result.success = false;
                    result.message = message;
                    return Ok(result);
                }
                result.success = BusinessTransaction.Instance.Duplicate(yeucau, model.maYCau, out message);
                result.message = message;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("yeucaukhaosatlai")]
        public IHttpActionResult YeuCauKhaoSatLai([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                var congvan = congvansrv.Getbykey(model.id);
                if (congvan.TrangThai > TrangThaiCongVan.PhanCongKS)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được yêu cầu khảo sát lại, yêu cầu đã phân công khảo sát";
                    result.success = false;
                    return Ok(result);
                }
                if (!congvansrv.CancelYeuCauKhaoSat(congvan))
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
