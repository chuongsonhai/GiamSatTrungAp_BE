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
    [RoutePrefix("api/BaoCaoChiTietctgsController")]
    public class BaoCaoChiTietctgsController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BaoCaoChiTietctgsController));
        [JwtAuthentication]
        [HttpPost]
        [Route("BaoCaoChiTietctgs")]
        public IHttpActionResult GetBaoCaoTHQuaHan(BienBanFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetListBaoCaoTHTCDN(request.maDViQly, request.keyword, null, request.status, fromDate, toDate);
                var listModel = new BaoCaoTHTCDNViewModel(list);
                result.data = listModel;
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
        [Route("BaoCaoChiTietctgs")]
        public IHttpActionResult ExportTHQuaHan(BienBanFilter request)
        {
            try
            {
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/TongHopQuaHan.xlsx";
                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetListBaoCaoTHQuaHan(request.maDViQly, request.keyword, null, request.status, fromDate, toDate);
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