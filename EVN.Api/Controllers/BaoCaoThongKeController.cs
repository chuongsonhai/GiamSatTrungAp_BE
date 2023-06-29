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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace EVN.Api.Controllers
{
    [RoutePrefix("api/baocaothongke")]
    public class BaoCaoThongKeController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BaoCaoThongKeController));
        // GET: BaoCaoThongKe
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
                DateTime synctime = DateTime.Today;
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromDate = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    toDate = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetThongKe(request.Filter.maDViQLy, request.Filter.keyword, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);
                var listModel = new List<YeuCauDataRequest>();
                foreach (var item in list)
                {
                    var model = new YeuCauDataRequest(item);
                    model.MaHinhThuc = "WEB EVNHANOI";
                    var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);
                    if(bbks != null)
                    {
                        model.CongSuat = bbks.TongCongSuat.ToString();
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
        [Route("export")]
        public IHttpActionResult Export(YeuCauFilter request)
        {
            try
            {
                int total = 0;
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ThongKe.xlsx";
                DateTime synctime = DateTime.Today;
                ICongVanYeuCauService service = IoC.Resolve<ICongVanYeuCauService>();
                IBienBanKSService bienBanKSService = IoC.Resolve<IBienBanKSService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetThongKeExport(request.maDViQLy, request.keyword, fromDate, toDate);
                var listModel = new List<YeuCauDataRequest>();
                foreach (var item in list)
                {
                    var model = new YeuCauDataRequest(item);
                    model.MaHinhThuc = "WEB EVNHANOI";
                    var bbks = bienBanKSService.GetbyYeuCau(item.MaYeuCau);
                    if (bbks != null)
                    {
                        model.CongSuat = bbks.TongCongSuat.ToString();
                    }
                    listModel.Add(model);
                }
                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    int row = 3;
                    int stt = 0;
                    foreach (var item in listModel)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenKhachHang;
                        colval++;

                        ws.Cells[row, colval].Value = item.DienThoai;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NgayYeuCau;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.DiaChiDungDien;
                        colval++;

                        ws.Cells[row, colval].Value = item.MaDViQLy;
                        colval++;


                        ws.Cells[row, colval].Value = item.CongSuat;
                        colval++;


                        ws.Cells[row, colval].Value = item.MaHinhThuc;
                        colval++;
                        row++;
                    }

                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }
    }
}