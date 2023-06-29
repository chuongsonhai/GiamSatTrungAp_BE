using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/bienbandn")]
    public class BienBanDNController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BienBanDNController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(BienBanRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                DateTime fromtime = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                DateTime totime = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<BienBanDNModel>();
                foreach (var item in list)
                {
                    data.Add(new BienBanDNModel(item));
                }
                result.total = total;
                result.data = data;
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
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();

                var yeucau = congvansrv.Getbykey(id);
                var item = service.GetbyMaYeuCau(yeucau.MaYeuCau);
                AsposeUtils aspose = new AsposeUtils();
                if (item != null && string.IsNullOrWhiteSpace(item.Data) && !string.IsNullOrWhiteSpace(item.DocPath))
                {
                    string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                    string pdfPath = aspose.ConvertWordToPDF(folder, item.DocPath);
                    if (!string.IsNullOrWhiteSpace(pdfPath))
                    {
                        item.Data = pdfPath;
                        service.Save(item);
                        service.CommitChanges();
                    }
                }
                if (item == null)
                {
                    var org = orgsrv.GetbyCode(yeucau.MaDViQLy);
                    item = new BienBanDN();

                    item.SoCongVan = yeucau.SoCongVan;
                    item.NgayCongVan = yeucau.NgayYeuCau;
                    item.MaYeuCau = yeucau.MaYeuCau;
                    item.MaDViQLy = yeucau.MaDViQLy;

                    item.EVNDonVi = org.orgName;
                    item.EVNDiaChi = org.address;
                    item.EVNMaSoThue = org.taxCode;
                    item.EVNDaiDien = org.daiDien;
                    item.EVNChucVu = org.chucVu;
                    item.EVNDienThoai = org.phone;
                    item.EVNTaiKhoan = org.soTaiKhoan;

                    item.MaKH = yeucau.MaKHang;
                    item.KHTen = yeucau.CoQuanChuQuan;
                    item.KHDaiDien = yeucau.NguoiYeuCau;
                    item.KHDiaChi = yeucau.DiaChiCoQuan;
                    item.KHDienThoai = yeucau.DienThoai;
                    item.KHMaSoThue = yeucau.MST;
                    item.KHTaiKhoan = yeucau.SoTaiKhoan;

                    item.DiaDiemXayDung = yeucau.DiaChiDungDien;
                    item.TenCongTrinh = yeucau.DuAnDien;
                    item.Data = "/bieumau/TTDN.pdf";
                }
                BienBanDNData model = new BienBanDNData();
                model.CongVanYeuCau = new YeuCauDauNoiData(yeucau);
                model.BienBanDN = new BienBanDNModel(item);                
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
        [Route("hoanthanh")]
        public IHttpActionResult HoanThanh([FromBody] ApproveModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();

                var item = service.Getbykey(model.id);

                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                result.success = service.Complete(item, model.deptId, model.staffCode, ngayHen);
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
        [Route("chuyentiep")]
        public IHttpActionResult ChuyenTiep([FromBody] ChuyenTiepModel model)
        {
            try
            {
                DateTime ngayHen = DateTime.ParseExact(model.NGAY_HEN, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                IChuyenTiepService ctiepservice = IoC.Resolve<IChuyenTiepService>();
                var item = service.Getbykey(model.ID);

                ChuyenTiep data = ctiepservice.GetbyMaYCau(item.MaLoaiYeuCau, item.MaYeuCau);
                if (data == null) data = new ChuyenTiep();
                data.MA_DVIQLY = item.MaDViQLy;
                data.MA_YCAU_KNAI = item.MaYeuCau;
                data.MA_DDO_DDIEN = item.MaKH;
                data.NDUNG_XLY = model.NDUNG_XLY;
                data.MA_CVIEC = model.MA_CVIEC;

                data.MA_BPHAN_NHAN = model.MA_BPHAN_NHAN;
                data.MA_NVIEN_NHAN = model.MA_NVIEN_NHAN;
                data.NGAY_HEN = ngayHen;
                data.MA_LOAI_YCAU = item.MaLoaiYeuCau;
                data.TRANG_THAI = 1;
                if (!ctiepservice.SaveChuyenTiep(item, data))
                {
                    return BadRequest();
                }
                var result = new ChuyenTiepModel(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("detail/{id}")]
        public IHttpActionResult Detail(int id)
        {
            IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
            var item = service.Getbykey(id);
            if (string.IsNullOrWhiteSpace(item.Data))
                return BadRequest();
            IRepository repository = new FileStoreRepository();
            byte[] pdfdata = repository.GetData(item.Data);
            if (pdfdata == null || pdfdata.Length == 0)
                return BadRequest();
            return Ok(pdfdata);
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("upload")]
        public IHttpActionResult Upload()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string data = httpRequest.Form["data"];
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IRepository repository = new FileStoreRepository();

                var model = JsonConvert.DeserializeObject<BienBanDNModel>(data);
                var yeucau = congvansrv.GetbyMaYCau(model.MaYeuCau);
                var item = service.GetbyMaYeuCau(model.MaYeuCau);
                if (item == null)
                    item = new BienBanDN();
                model.ToEntity(item);

                item.MaDViQLy = yeucau.MaDViQLy;
                item.MaYeuCau = yeucau.MaYeuCau;
                item.MaLoaiYeuCau = yeucau.MaLoaiYeuCau;
                item.MaDDoDDien = yeucau.MaDDoDDien;
                item.SoCongVan = yeucau.SoCongVan;
                item.NgayCongVan = yeucau.NgayYeuCau;
                item.MaKH = yeucau.MaKHang;

                //Upload Image
                var postedFile = httpRequest.Files["File"];
                if (postedFile == null || postedFile.ContentLength == 0)
                    throw new Exception("Chưa có file đính kèm");

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(postedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                }

                string loaiFile = Path.GetExtension(postedFile.FileName);
                string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";
                if (loaiFile.ToUpper().Contains("DOC"))
                {
                    item.DocPath = repository.Store(folder, fileData, item.DocPath, loaiFile.Replace(".", ""));
                    item.Data = String.Empty;
                    log.Error(item.DocPath);
                }
                if (loaiFile.ToUpper().Contains("PDF"))
                    item.Data = repository.Store(folder, fileData, item.Data);
                string message = "";
                result.success = service.Save(item, out message);
                result.message = message;
                result.data = new BienBanDNModel(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }

        [HttpPost]
        [JwtAuthentication]
        [Route("notify")]
        public IHttpActionResult Notify(BienBanDNModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IRepository repository = new FileStoreRepository();
                var yeucau = congvansrv.GetbyMaYCau(model.MaYeuCau);
                var item = service.GetbyMaYeuCau(model.MaYeuCau);
                if (item.TrangThai > 0)
                {
                    result.success = false;
                    result.message = "Dự thảo đã được gửi khách hàng, vui lòng chờ xác nhận";
                    return Ok(result);
                }
                model.ToEntity(item);
                item.MaDViQLy = yeucau.MaDViQLy;
                item.MaLoaiYeuCau = yeucau.MaLoaiYeuCau;
                item.MaDDoDDien = yeucau.MaDDoDDien;
                item.SoCongVan = yeucau.SoCongVan;
                item.NgayCongVan = yeucau.NgayYeuCau;
                item.MaKH = yeucau.MaKHang;

                VanBanRequest request = new VanBanRequest(model.SoBienBan, item.NgayBienBan);
                VanBanResponse response = DOfficeUtils.GetDocument(request);
                if (response == null)
                {
                    result.success = false;
                    result.message = "Số hoặc ngày văn bản không đúng";
                    return Ok(result);
                }
                byte[] pdfdata = Convert.FromBase64String(response.DATA);
                if (pdfdata != null)
                {
                    string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                    item.Data = repository.Store(folder, pdfdata, item.Data);
                }

                string message = "";
                result.success = false;
                if (service.Notify(item, out message))
                {
                    result.success = true;
                    if (model.CusSigned)
                    {
                        var bienbandn = service.Getbykey(item.ID);
                        result.success = service.Confirm(bienbandn, pdfdata);
                    }
                }
                result.message = message;
                result.data = new BienBanDNModel(item);
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

        [HttpPost]
        [JwtAuthentication]
        [Route("confirm")]
        public IHttpActionResult Confirm(BienBanDNModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                IRepository repository = new FileStoreRepository();
                var item = service.GetbyMaYeuCau(model.MaYeuCau);                                
                byte[] pdfdata = repository.GetData(item.Data);

                string message = "";
                result.success = false;
                result.success = service.Confirm(item, pdfdata);
                result.message = message;
                result.data = new BienBanDNModel(item);
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

        [HttpPost]
        [JwtAuthentication]
        [Route("getpdf")]
        public IHttpActionResult GetPdf(BienBanDNModel model)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IBienBanDNService service = IoC.Resolve<IBienBanDNService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                IRepository repository = new FileStoreRepository();
                var yeucau = congvansrv.GetbyMaYCau(model.MaYeuCau);
                var item = service.GetbyMaYeuCau(model.MaYeuCau);
                if (item.TrangThai > 0)
                {
                    log.Error("Dự thảo đã được gửi khách hàng, vui lòng chờ xác nhận");
                    result.success = false;
                    result.message = "Dự thảo đã được gửi khách hàng, vui lòng chờ xác nhận";
                    return Ok(result);
                }
                model.ToEntity(item);

                VanBanRequest request = new VanBanRequest(model.SoBienBan, item.NgayBienBan);
                VanBanResponse response = DOfficeUtils.GetDocument(request);
                if (response == null)
                {
                    log.Error("Không lấy được file từ D-Office");
                    result.success = false;
                    result.message = "Số hoặc ngày văn bản không đúng";
                    return Ok(result);
                }
                item.Data = String.Empty;
                byte[] pdfdata = Convert.FromBase64String(response.DATA);
                string folder = $"{item.MaDViQLy}/{item.MaYeuCau}";
                item.Data = repository.Store(folder, pdfdata, item.Data);

                result.success = true;
                result.data = new BienBanDNModel(item);
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
}
