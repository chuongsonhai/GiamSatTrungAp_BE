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
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.Http;
using System.Xml.Xsl;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/hopdongdien")]
    public class HopDongDienController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(HopDongDienController));

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
                IHopDongService service = IoC.Resolve<IHopDongService>();
                var list = service.GetbyFilter(request.Filter.maDViQly, request.Filter.maYCau, request.Filter.keyword, request.Filter.status, fromtime, totime, pageindex, request.Paginator.pageSize, out total);

                var data = new List<HopDongModel>();
                foreach (var item in list)
                {
                    data.Add(new HopDongModel(item));
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
        [HttpPost]
        [Route("detail")]
        public IHttpActionResult Detail(TTYeuCauRequest request)
        {
            try
            {
                IHopDongService service = IoC.Resolve<IHopDongService>();
                ICongVanYeuCauService ycausrv = IoC.Resolve<ICongVanYeuCauService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IRepository repository = new FileStoreRepository();

                var item = service.GetbyMaYCau(request.maYCau);
                if (item != null && !string.IsNullOrWhiteSpace(item.Data))
                    return Ok(repository.GetData(item.Data));

                string maDViQly = request.maDViQLy;
                string maYCau = request.maYCau;
                string maLoaiHSo = LoaiHSoCode.HD_NSH;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                if (pdfdata != null && pdfdata.Length > 0)
                {
                    IHoSoGiayToService hsoservice = IoC.Resolve<IHoSoGiayToService>();
                    var hoso = hsoservice.Get(p => p.MaDViQLy == maDViQly && p.MaYeuCau == maYCau && p.LoaiHoSo == maLoaiHSo);
                    if (item == null) item = new HopDong();
                    try
                    {
                        var org = orgsrv.GetbyCode(request.maDViQLy);
                        var yeucau = ycausrv.GetbyMaYCau(request.maYCau);
                        item = new HopDong();
                        item.MaYeuCau = yeucau.MaYeuCau;
                        item.MaDViQLy = yeucau.MaDViQLy;
                        item.KHMa = yeucau.MaKHang;

                        item.DonVi = org.orgName;
                        item.DiaChi = org.address;
                        item.MaSoThue = org.taxCode;
                        item.DaiDien = org.daiDien;
                        item.ChucVu = org.chucVu;
                        item.DienThoai = org.phone;
                        item.Email = org.email;
                        item.KHTen = !string.IsNullOrWhiteSpace(yeucau.CoQuanChuQuan) ? yeucau.CoQuanChuQuan : yeucau.NguoiYeuCau;
                        item.KHDaiDien = yeucau.NguoiYeuCau;

                        string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}/HDNSH";
                        item.Data = repository.Store(folder, pdfdata);

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
                        hoso.TrangThai = 0;
                        hoso.Data = item.Data;
                        service.BeginTran();
                        hsoservice.Save(hoso);
                        service.Save(item);
                        service.CommitTran();
                        return Ok(pdfdata);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        service.RolbackTran();
                        return NotFound();
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IHopDongService service = IoC.Resolve<IHopDongService>();
                IYCauNghiemThuService congvansrv = IoC.Resolve<IYCauNghiemThuService>();
                IBienBanDNService ttdnsrv = IoC.Resolve<IBienBanDNService>();
                IOrganizationService orgsrv = IoC.Resolve<IOrganizationService>();
                IDvTienTrinhService ttrinhservice = IoC.Resolve<IDvTienTrinhService>();

                var yeucau = congvansrv.Getbykey(id);
                var thoathuandn = ttdnsrv.GetbyMaYeuCau(yeucau.MaYeuCau);
                var hopdong = service.GetbyMaYCau(yeucau.MaYeuCau);

                if (hopdong == null)
                {
                    var org = orgsrv.GetbyCode(yeucau.MaDViQLy);
                    hopdong = new HopDong();
                    hopdong.MaYeuCau = yeucau.MaYeuCau;
                    hopdong.MaDViQLy = yeucau.MaDViQLy;
                    hopdong.KHMa = yeucau.MaKHang;

                    hopdong.DonVi = org.orgName;
                    hopdong.DiaChi = org.address;
                    hopdong.MaSoThue = org.taxCode;
                    hopdong.DaiDien = org.daiDien;
                    hopdong.ChucVu = org.chucVu;
                    hopdong.DienThoai = org.phone;
                    hopdong.Email = org.email;
                    hopdong.TrangThai = (int)TrangThaiBienBan.DaDuyet;
                    hopdong.KHTen = !string.IsNullOrWhiteSpace(yeucau.CoQuanChuQuan) ? yeucau.CoQuanChuQuan : yeucau.NguoiYeuCau;
                    hopdong.KHDaiDien = yeucau.NguoiYeuCau;
                    service.CreateNew(hopdong);
                    service.CommitChanges();
                }
                HopDongModel model = new HopDongModel(hopdong);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("getpdf/{id}")]
        public IHttpActionResult GetPdf(int id)
        {
            try
            {
                IHopDongService service = IoC.Resolve<IHopDongService>();
                IRepository repository = new FileStoreRepository();

                var item = service.Getbykey(id);
                if (item == null) return NotFound();
                
                string maDViQly = item.MaDViQLy;
                string maYCau = item.MaYeuCau;
                string maLoaiHSo = LoaiHSoCode.HD_NSH;
                ICmisProcessService cmisProcess = new CmisProcessService();
                byte[] pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                if(pdfdata == null || pdfdata.Length == 0)
                {
                    maLoaiHSo = LoaiHSoCode.HD_NH;
                    pdfdata = cmisProcess.GetData(maDViQly, maYCau, maLoaiHSo);
                }
                if (pdfdata != null && pdfdata.Length > 0)
                {
                    service.UpdatebyCMIS(item, pdfdata);
                    return Ok(pdfdata);
                }
                if (!string.IsNullOrWhiteSpace(item.Data))
                    return Ok(repository.GetData(item.Data));
                return Ok(repository.GetData("/bieumau/BBTreoThao.pdf"));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("notify")]
        public IHttpActionResult Notify([FromBody] ApproveModel model)
        {
            try
            {
                DateTime ngayHen = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(model.ngayHen))
                    ngayHen = DateTime.ParseExact(model.ngayHen, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                IHopDongService service = IoC.Resolve<IHopDongService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();
                var congviec = cvservice.Getbykey(model.maCViec);
                model.noiDung = !string.IsNullOrWhiteSpace(model.noiDung) ? model.noiDung : congviec.TEN_CVIEC;

                var item = service.Getbykey(model.id);

                if (!service.Notify(item, model.maCViec, model.deptId, model.staffCode, ngayHen, model.noiDung, out string message))
                    return BadRequest();
                HopDongModel result = new HopDongModel(item);
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
        [Route("genpdf")]
        public IHttpActionResult GenPdf(PhuLucRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                var temp = TemplateManagement.GetTemplate(request.loaiHoSo);
                if (temp == null)
                {
                    log.Error(request.loaiHoSo);
                    result.message = "Sai mã loại hồ sơ";
                    result.success = false;
                    return Ok(result);
                }
                StringWriter sw = new StringWriter();
                XslCompiledTransform xct = new XslCompiledTransform();
                xct.Load(new XmlTextReader(new StringReader(temp.XsltData)));
                xct.Transform(request.xmlData, null, sw);
                string html = sw.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.Zoom = 1.6f;
                htmlToPdf.Margins.Top = 5;
                htmlToPdf.Margins.Bottom = 5;
                htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
                htmlToPdf.CustomWkHtmlPageArgs = "--enable-smart-shrinking";

                var pdf = htmlToPdf.GeneratePdf(html);
                result.data = Convert.ToBase64String(pdf);
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
        [Route("upload")]
        public IHttpActionResult Upload(PhuLucDataRequest request)
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

                byte[] fileData = Convert.FromBase64String(request.Base64Data);
                string folder = $"{yeucau.MaDViQLy}/{yeucau.MaYeuCau}";

                var hoSo = service.GetHoSoGiayTo(yeucau.MaDViQLy, request.maYeuCau, request.loaiHoSo);
                if (hoSo == null) hoSo = new HoSoGiayTo();
                hoSo.TrangThai = 1;
                hoSo.MaYeuCau = yeucau.MaYeuCau;
                hoSo.MaDViQLy = yeucau.MaDViQLy;
                hoSo.LoaiHoSo = request.loaiHoSo;
                hoSo.TenHoSo = request.tenHoSo;
                hoSo.Data = repository.Store(folder, fileData, hoSo.Data);
                service.Save(hoSo);
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
}
