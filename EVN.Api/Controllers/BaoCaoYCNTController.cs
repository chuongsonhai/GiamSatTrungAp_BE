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
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/baocaoycnt")]
    public class BaoCaoYCNTController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(BaoCaoYCNTController));

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

                var list = service.GetListBaoCaoNT(request.Filter.maDViQLy, request.Filter.keyword, request.Filter.khachhang, request.Filter.status, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);

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
        public IHttpActionResult Export(YeuCauFilter request)
        {
            try
            {
           
                int total = 0;
                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetListBaoCaoNT(request.maDViQLy, request.keyword, request.khachhang, request.status, fromDate, toDate, 0, 0, out total);


                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ChiTietKTDK.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    int row = 5;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.MaDViQLy;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenKH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenCT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DCCT;
                        colval++;

                        ws.Cells[row, colval].Value = item.MaYC;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongCS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongChieuDaiDD;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianTNHS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianTLHS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TroNgaiTNHS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianChuyenHSSangKT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayTNHS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianKTThucTe;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TroNgaiKT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianYeuCauKHHoanThanhTonTai;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianLapVaDuyetBBKT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianChuyenBBKTDenKH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianTNHS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TroNgaiNT;
                        colval++;

                        ws.Cells[row, colval].Value = item.ThoiGianNTDDVaKyHD;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayNTVaKyHD;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;
                        ws.Cells[row, colval].Value = item.TongSoNgayYCNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
