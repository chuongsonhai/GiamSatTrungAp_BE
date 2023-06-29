using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/chitiettcdnquahan")]
    public class BaoCaoChiTietTCDNQuaHanController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BaoCaoChiTietTCDNQuaHanController));

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
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.Filter.fromdate))
                    fromDate = DateTime.ParseExact(request.Filter.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.Filter.todate))
                    toDate = DateTime.ParseExact(request.Filter.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetListBaoCaoChiTietQuaHan(request.Filter.maDViQLy, request.Filter.keyword, request.Filter.khachhang, -1, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);

                result.total = total;
                result.data = list;
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
        [Route("export")]
        public IHttpActionResult ExportTHQuaHan(YeuCauFilter request)
        {
            try
            {
                int total = 0;
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ChiTietQuaHan.xlsx";
                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetListBaoCaoChiTietQuaHan(request.maDViQLy, request.keyword, request.khachhang, -1, fromDate, toDate, 0, 0, out total);
                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
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
