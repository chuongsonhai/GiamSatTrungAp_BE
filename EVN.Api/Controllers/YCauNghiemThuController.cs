using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
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
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/ycaunghiemthu")]
    public class YCauNghiemThuController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(YCauNghiemThuController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(YeuCauFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanKTService bienBanKTService = IoC.Resolve<IBienBanKTService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ISystemConfigService cfgservice = IoC.Resolve<ISystemConfigService>();                

                var cskhcfg = cfgservice.GetbyCode("TTRINH_SYNC");
                //if (cskhcfg != null && cskhcfg.Value == "1")
                //{
                //    service.Sync();
                //    service.SyncPMIS();
                //}

                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromDate = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    toDate = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                request.Filter.keyword = !string.IsNullOrWhiteSpace(request.Filter.keyword) ? request.Filter.keyword.Trim() : request.Filter.keyword;
                var list = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.keyword, request.Filter.khachhang, request.Filter.status, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<YeuCauNghiemThuData>();
                foreach (var item in list)
                {
                    var model = new YeuCauNghiemThuData(item);
                    var bbkt = bienBanKTService.GetbyMaYCau(item.MaYeuCau);
                    if (bbkt != null)
                    {
                        model.TroNgai = bbkt.TroNgai;
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
                result.data = new List<YeuCauNghiemThuData>();
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
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                ICongViecService congViecService = IoC.Resolve<ICongViecService>();
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
                    string tenCV = congViecService.Getbykey(first.MaCViec).MA_CVIEC;
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
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                var item = service.Getbykey(id);
                var ttdn = ttdnservice.Get(p => p.SoBienBan == item.SoThoaThuanDN && p.MaYeuCau == item.MaYeuCau);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }
                item.MaDViQLy = ttdn.MaDViQLy;
                item.MaYeuCau = ttdn.MaYeuCau;
                item.MaDDoDDien = ttdn.MaDDoDDien;
                item.DuAnDien = ttdn.TenCongTrinh;
                item.DiaChiDungDien = ttdn.DiaDiemXayDung;
                item.MaKHang = ttdn.MaKH;
                item.SoThoaThuanDN = ttdn.SoBienBan;
                item.NgayThoaThuan = ttdn.NgayLap;
                if (string.IsNullOrWhiteSpace(item.MaKHang))
                {
                    ICongVanYeuCauService cvservice = IoC.Resolve<ICongVanYeuCauService>();
                    var cvdn = cvservice.SyncData(item.MaYeuCau);
                    item.MaKHang = cvdn.MaKHang;
                    service.Save(item);
                    service.CommitChanges();
                }
                if (string.IsNullOrWhiteSpace(item.Data))
                {                    
                    item.Data = item.GetPdf(true);
                    service.Save(item);
                    service.CommitChanges();
                }
                YeuCauNghiemThuData model = new YeuCauNghiemThuData(item);
                model.PdfBienBanDN = ttdn.Data;
                model.GiaoB4 = item.MaDViQLy == "PD";
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] YCauNThuRequest model)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                log.Error(JsonConvert.SerializeObject(model));
                IMaDichVuService dvuservice = IoC.Resolve<IMaDichVuService>();
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                IDvTienTrinhService tientrinhsrv = IoC.Resolve<IDvTienTrinhService>();

                var maDVu = dvuservice.Getbykey(model.MaYeuCau);
                if (maDVu == null)
                {
                    log.Error($"Chưa có mã dịch vụ tương ứng: {model.MaYeuCau}");
                    result.message = "Mã yêu cầu hoặc mã xác nhận không hợp lệ";
                    result.success = false;
                    return Ok(result);
                }

                if (maDVu.ID_WEB != model.MaDichVu)
                {
                    log.Error($"Chưa có mã ID_WEB: {model.MaYeuCau}");
                    result.message = "Mã yêu cầu hoặc mã xác nhận không hợp lệ";
                    result.success = false;
                    return Ok(result);
                }

                var item = service.GetbyMaYCau(model.MaYeuCau);
                if (item == null)
                    item = new YCauNghiemThu();
                var ttdn = ttdnservice.GetbyMaYeuCau(model.MaYeuCau);
                if (ttdn == null || ttdn.TrangThai != (int)TrangThaiBienBan.HoanThanh)
                {
                    log.Error("Chưa hoàn thiện thỏa thuận đấu nối");
                    result.success = false;
                    result.message = "Chưa hoàn thiện thỏa thuận đấu nối";
                    return Ok(result);
                }
                item.TrangThai = TrangThaiNghiemThu.MoiTao;
                item.SoThoaThuanDN = ttdn.SoBienBan;
                item.NgayThoaThuan = ttdn.NgayLap;

                item.MaCViec = "KDN";
                item.MaLoaiYeuCau = ttdn.MaLoaiYeuCau;
                item.MaYeuCau = model.MaYeuCau;
                item.MaDDoDDien = ttdn.MaDDoDDien;
                item.MaDViQLy = ttdn.MaDViQLy;

                item.NguoiYeuCau = model.NguoiYeuCau;
                item.NgayYeuCau = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(model.NgayYeuCau))
                    item.NgayYeuCau = DateTime.ParseExact(model.NgayYeuCau, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                item.DiaChi = ttdn.KHDiaChi;

                item.MaKHang = ttdn.MaKH;
                item.CoQuanChuQuan = ttdn.KHTen;
                item.MaSoThue = ttdn.KHMaSoThue;

                item.DiaChiCoQuan = ttdn.KHDiaChi;
                item.DienThoai = ttdn.KHDienThoai;
                item.DuAnDien = ttdn.TenCongTrinh;
                item.DiaChiDungDien = ttdn.DiaDiemXayDung;
                item.NoiDungYeuCau = model.NoiDungYeuCau;

                string message = "";
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
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IHoSoGiayToService hsogtosrv = IoC.Resolve<IHoSoGiayToService>();

                var item = service.Get(p => p.Fkey == model.Code);
                if (item == null)
                {
                    log.Error("Không tìm thấy pdf");
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
        [Route("approve")]
        public IHttpActionResult Approve([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string message = "";
                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                IHopDongService hdongsrv = IoC.Resolve<IHopDongService>();

                var congviec = cvservice.Getbykey(model.maCViec);
                model.noiDung = congviec.TEN_CVIEC;
                var item = service.Getbykey(model.id);
                
                var ttdn = ttdnservice.GetbyNo(item.SoThoaThuanDN, item.MaYeuCau);
                if (ttdn == null)
                {
                    result.message = "Số thỏa thuận đấu nối không hợp lệ.";
                    result.success = false;
                    return Ok(result);
                }

                //string maLoaiHSo = LoaiHSoCode.HD_NSH;
                //ICmisProcessService cmisProcess = new CmisProcessService();
                //byte[] pdfdata = cmisProcess.GetData(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                //if (pdfdata == null || pdfdata.Length == 0)
                //{                    
                //    result.message = "Chưa tạo dự thảo hợp đồng mua bán điện trên CMIS.";
                //    result.success = false;
                //    return Ok(result);
                //}

                if (!service.Approve(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung, out message))
                {
                    result.message = message;
                    result.success = false;
                    return Ok(result);
                }    
                YeuCauNghiemThuData data = new YeuCauNghiemThuData(item);
                data.PdfBienBanDN = ttdn.Data;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                result.success = false;
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("yeucaukiemtra")]
        public IHttpActionResult YeuCauKiemTra([FromBody] ApproveModel model)
        {
            try
            {
                string message = "";

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnservice = IoC.Resolve<IBienBanDNService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();

                var congviec = cvservice.Getbykey(model.maCViec);
                model.noiDung = congviec.TEN_CVIEC;

                var item = service.Getbykey(model.id);
                var ttdn = ttdnservice.GetbyNo(item.SoThoaThuanDN, item.MaYeuCau);
                if (ttdn == null)
                {
                    throw new Exception("Số thỏa thuận đấu nối không hợp lệ.");
                }

                //string maLoaiHSo = LoaiHSoCode.HD_NSH;
                //ICmisProcessService cmisProcess = new CmisProcessService();
                //byte[] pdfdata = cmisProcess.GetData(item.MaDViQLy, item.MaYeuCau, maLoaiHSo);
                //if (pdfdata == null || pdfdata.Length == 0)
                //    return BadRequest("Chưa tạo dự thảo hợp đồng mua bán điện trên CMIS.");

                if (!service.YeuCauKiemTra(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung, out message))
                    return BadRequest();
                YeuCauNghiemThuData result = new YeuCauNghiemThuData(item);
                result.PdfBienBanDN = ttdn.Data;
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
        [Route("yeucaukiemtralai")]
        public IHttpActionResult YeuCauKiemTraLai([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                var congvan = congvansrv.Getbykey(model.id);
                if (congvan.TrangThai > TrangThaiNghiemThu.PhanCongKT)
                {
                    log.Error(congvan.TrangThai);
                    result.message = "Không được yêu cầu kiêm tra lại, yêu cầu đã phân công kiểm tra";
                    result.success = false;
                    return Ok(result);
                }
                if (!congvansrv.CancelYeuCauKiemTra(congvan))
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

        [JwtAuthentication]
        [HttpPost]
        [Route("listhso")]
        public IHttpActionResult ListHSo(TTYeuCauRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string[] doccodes = new string[] { LoaiHSoCode.TL_TKKT, LoaiHSoCode.TL_HDVH, LoaiHSoCode.TL_BBNT, LoaiHSoCode.TL_DKDD };

                IHoSoGiayToService service = IoC.Resolve<IHoSoGiayToService>();
                IList<HoSoGiayTo> model = service.Query.Where(p => p.MaDViQLy == request.maDViQLy && p.MaYeuCau == request.maYCau && doccodes.Contains(p.LoaiHoSo)).ToList();
                result.data = model;
                result.success = true;
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
        [Route("trahoso")]
        public IHttpActionResult TraHoSo(CancelModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IYCauNghiemThuService service = IoC.Resolve<IYCauNghiemThuService>();
                var item = service.GetbyMaYCau(model.maYCau);
               
                item.LyDoHuy = model.noiDung;
                result.success = service.Cancel(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                return Ok(result);
            }
        }
    }
}