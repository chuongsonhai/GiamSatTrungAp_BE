using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using FX.Core;
using log4net;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Globalization;
using System.IO;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(ReportController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(ReportRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IReportService service = IoC.Resolve<IReportService>();
                var list = service.ListbyQuater(request.Filter.maDViQLy, request.Filter.quater, request.Filter.year, pageindex, request.Paginator.pageSize, out total);

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
        public IHttpActionResult Export(ReportFilter filter)
        {
            try
            {
                int total = 0;
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ReportQuater.xlsx";
                IReportService service = IoC.Resolve<IReportService>();
                var list = service.ListbyQuater(filter.maDViQLy, filter.quater, filter.year, 0, 0, out total);
                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    int row = 2;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.TONG_TGIAN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_TNHAN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_KSAT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TEN_NGUOIYCAU;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_YCAU_KNAI;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_BDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_DT_TDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_TTX_TDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGIAN_DNOI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_TH_TVB;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_NV_TVB;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_NT_TVB;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGIAN_NTHU;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGIAN_KSAT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.TONG_TGIAN_GIAIQUYET;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGIAN_TNHAN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_CHUYENKS;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_TNHANYCAU;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TEN_DVIQLY;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_HTAT_YC;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_DVIQLY;
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

        [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaotonghop")]
        public IHttpActionResult GetBaoCaoTongHop(BienBanFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                var list = service.GetBaoCaoTongHop(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                result.data = baoCaoTongHopModel;
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
        [Route("exporttonghop")]
        public IHttpActionResult ExportTongHop(BienBanFilter request)
        {
            try
            {
                IReportService service = IoC.Resolve<IReportService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/TongHop.xlsx";
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                var list = service.GetBaoCaoTongHop(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    string title = $"BÁO CÁO CHỈ TIÊU TIẾP CẬN ĐIỆN NĂNG TỪ NGÀY {fromDate.ToString("dd/MM/yyyy")} ĐẾN NGÀY {toDate.ToString("dd/MM/yyyy")}";
                    if(!request.isHoanTat)
                        title = $"BÁO CÁO CÁC CÔNG TRÌNH ĐANG THỰC HIỆN TỪ NGÀY {fromDate.ToString("dd/MM/yyyy")} ĐẾN NGÀY {toDate.ToString("dd/MM/yyyy")}";

                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 1].Value = title;
                    ws.Cells[7, 3].Value = baoCaoTongHopModel.TongSoCTTrongThang;
                    ws.Cells[7, 4].Value = baoCaoTongHopModel.TongTGTrongThang;
                    ws.Cells[7, 5].Value = baoCaoTongHopModel.TongSoCTLuyKe;
                    ws.Cells[7, 6].Value = baoCaoTongHopModel.TongTGLuyKe;
                    int row = 8;
                    int stt = 0;
                    foreach (var item in baoCaoTongHopModel.baoCaoTongHopChiTietDataModels)
                    {
                        stt++;
                        int colval = 2;
                        ws.Cells[row, colval].Value = item.TenDonVi;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoCTTrongThang;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGTrongThang;

                        colval++;

                        ws.Cells[row, colval].Value = item.SoCTLuyKe;

                        colval++;

                        ws.Cells[row, colval].Value = item.TGLuyKe;

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

        [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaochitietthang")]
        public IHttpActionResult GetBaoCaoChiTietThang(BienBanFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                IReportService service = IoC.Resolve<IReportService>();
                if (request.thang == 0)
                {
                    request.thang = DateTime.Now.Month;
                }
                if (request.nam == 0)
                {
                    request.nam = DateTime.Now.Year;
                }


                var fromDate = new DateTime(request.nam,request.thang,1);
                var toDate = new DateTime(request.nam, request.thang, DateTime.DaysInMonth(request.nam, request.thang));
               
                var list = service.GetBaoCaoChiTiet(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                result.data = baoCaoTongHopModel;
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
        [Route("exportchitietthang")]
        public IHttpActionResult ExportChiTietThang(BienBanFilter request)
        {
            try
            {
                IReportService service = IoC.Resolve<IReportService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ChiTietThang.xlsx";
                if (request.thang == 0)
                {
                    request.thang = DateTime.Now.Month;
                }
                if (request.nam == 0)
                {
                    request.nam = DateTime.Now.Year;
                }


                var fromDate = new DateTime(request.nam, request.thang, 1);
                var toDate = new DateTime(request.nam, request.thang, DateTime.DaysInMonth(request.nam, request.thang));

                var list = service.GetBaoCaoChiTiet(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 1].Value = "BÁO CÁO CÁC CHỈ TIÊU TIẾP CẬN ĐIỆN NĂNG TỪ " + fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year + " đến " + toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                    int row = 7;
                    int colTong = 3;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongSoCTTrongThang;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGTiepNhanTrongThang;

                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGKhaoSatTrongThang;

                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGTTDNTrongThang;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGNTTrongThang;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGTrongThang;

                    row++;
                    int stt = 0;
                    foreach (var item in baoCaoTongHopModel.baoCaoTongHopChiTietDataModels)
                    {
                        stt++;
                        int colval = 2;
                        ws.Cells[row, colval].Value = item.TenDonVi;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoCTTrongThang;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGTiepNhanTrongThang;

                        colval++;

                        ws.Cells[row, colval].Value = item.TGKhaoSatTrongThang;

                        colval++;

                        ws.Cells[row, colval].Value = item.TGTTDNTrongThang;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGNTTrongThang;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGTrongThang;

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

        [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaochitietluyke")]
        public IHttpActionResult GetBaoCaoChiTietLuyKe(BienBanFilter request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IReportService service = IoC.Resolve<IReportService>();

                DateTime totime = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);

                var list = service.GetBaoCaoChiTietLuyKe(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                result.data = baoCaoTongHopModel;
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
        [Route("exportchitietluyke")]
        public IHttpActionResult ExportChiTietLuyKe(BienBanFilter request)
        {
            try
            {
                IReportService service = IoC.Resolve<IReportService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ChiTietLuyKe.xlsx";
                DateTime totime = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                DateTime ngaydaunam = new DateTime(totime.Year, 1, 1);

                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.fromdate))
                    fromDate = DateTime.ParseExact(request.fromdate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(request.todate))
                    toDate = DateTime.ParseExact(request.todate, DateTimeParse.Format, null, System.Globalization.DateTimeStyles.None);
                var list = service.GetBaoCaoChiTietLuyKe(fromDate, toDate, request.isHoanTat);
                BaoCaoTongHopModel baoCaoTongHopModel = new BaoCaoTongHopModel(list);

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 1].Value = "BÁO CÁO CÁC CHỈ TIÊU TIẾP CẬN ĐIỆN NĂNG TỪ " + ngaydaunam.Day + "/" + ngaydaunam.Month + "/" + ngaydaunam.Year + " đến " + totime.Day + "/" + totime.Month + "/" + totime.Year;
                    ws.Cells[5, 3].Value = "Lũy kế tính đến " + totime.Day + "/" + totime.Month + "/" + totime.Year;
                    int row = 7;
                    int colTong = 3;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongSoCTLuyKe;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGTiepNhanLuyKe;

                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGKhaoSatLuyKe;

                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGTTDNLuyKe;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGNTLuyKe;
                    colTong++;

                    ws.Cells[row, colTong].Value = baoCaoTongHopModel.TongTGLuyKe;

                    row++;
                    int stt = 0;
                    foreach (var item in baoCaoTongHopModel.baoCaoTongHopChiTietDataModels)
                    {
                        stt++;
                        int colval = 2;
                        ws.Cells[row, colval].Value = item.TenDonVi;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoCTLuyKe;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGTiepNhanLuyKe;

                        colval++;

                        ws.Cells[row, colval].Value = item.TGKhaoSatLuyKe;

                        colval++;

                        ws.Cells[row, colval].Value = item.TGTTDNLuyKe;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGNTLuyKe;
                        colval++;

                        ws.Cells[row, colval].Value = item.TGLuyKe;

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

        [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaottdn")]
        public IHttpActionResult GetBaoCaoTTDN(YeuCauFilterRequest request)
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

                var list = service.GetListBaoCaoTTDN(request.Filter.maDViQLy, request.Filter.keyword, request.Filter.khachhang, request.Filter.status, fromDate, toDate, pageindex, request.Paginator.pageSize, out total);

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
        [Route("getbaocaothtcdn")]
        public IHttpActionResult GetBaoCaoTHTCDN(BienBanFilter request)
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
        [Route("exportbaocaothtcdn")]
        public IHttpActionResult ExportBaoCaoTHTCDN(BienBanFilter request)
        {
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


                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/THTCDN.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    int row = 5;
                    int stt = 0;
                    foreach (var item in listModel.ListItem)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenDV;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanTTDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiTTDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhTTDN;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayThucHienTBTTDN;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayThucHienTBKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.SoNgayThucHienTBNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoNgayTB;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;



                        row++;
                    }
                    int colvalT = 1;
                    ws.Cells[row, colvalT].Value = "";
                    colvalT++;

                    ws.Cells[row, colvalT].Value = "Tổng Công Ty";
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanTTDN;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiTTDN;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhTTDN;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.SoNgayThucHienTBTTDN;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.SoNgayThucHienTBKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.SoNgayThucHienTBNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoNgayTB;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }



        [JwtAuthentication]
        [HttpPost]
        [Route("exportbaocaothkq")]
        public IHttpActionResult ExportBaoCaoTHKQ(BienBanFilter request)
        {
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


                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/THKQ.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    int row = 5;
                    int stt = 0;
                    foreach (var item in listModel.ListItem)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenDV;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanTTDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiTTDN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhTTDN;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTChuaHoanThanhTTDN;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTChuaHoanThanhKTDK;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTTiepNhanNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTCoTroNgaiNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTDaHoanThanhNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoCTChuaHoanThanhNT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;
                        row++;
                    }
                    int colvalT = 1;
                    ws.Cells[row, colvalT].Value = "";
                    colvalT++;

                    ws.Cells[row, colvalT].Value = "Tổng Công Ty";
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanTTDN;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiTTDN;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhTTDN;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTChuaHoanThanhTTDN;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTChuaHoanThanhKTDK;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTTiepNhanNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTCoTroNgaiNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTDaHoanThanhNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;

                    ws.Cells[row, colvalT].Value = listModel.TongSoCTChuaHoanThanhNT;
                    ws.Cells[row, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colvalT++;
                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        //Báo cáo tổng hợp tiến độ (excel)
        //[JwtAuthentication]
        [HttpPost]
        [Route("exportbaocaotonghoptiendo")]
        public IHttpActionResult ExportBaoCaoTongHopTienDo(BaocaoTHTienDo request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/BaoCaoTongHopTienDo.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);

                var list = service.GetBaoCaotonghoptiendo(request.Filterbctd.maDViQly, request.Filterbctd.TenLoaiCanhBao, request.Filterbctd.fromdate, request.Filterbctd.todate);
                // var list = service.GetSoLuongGui(model.Filterdashboardcanhbao.fromdate, model.Filterdashboardcanhbao.todate);
                string title = $"BÁO CÁO TỔNG HỢP CÔNG TÁC GIÁM SÁT TIẾN ĐỘ GIẢI QUYẾT CẤP ĐIỆN TRUNG ÁP TỪ NGÀY {request.Filterbctd.fromdate} ĐẾN NGÀY {request.Filterbctd.todate}";
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    ws.Cells[2, 1].Value = title;

                    int row = 10;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.maDvi;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.CB_TONG;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.CB_SOCBLAN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.CB_CBTRONGAI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.CB_CBDVI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_DNN_TONG;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_DNN_TYLE;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_DNN_TRONGAI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_DNN_CHAM;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_KH_TONG;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_KH_TYLE;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_KH_CBTRONGAI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_KH_CBCHAM;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_LOI_TONG;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_LOI_TYLE;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_LOI_CBTRONGAI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NN_LOI_CBCHAM;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;
                        row++;

                    }
                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //Báo cáo tổng hợp tiến độ (get)
       // [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaotonghoptiendo")]
        public IHttpActionResult GetBaoCaoTonghopTienDo(BaocaoTHTienDo request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                ICanhBaoService  service = IoC.Resolve<ICanhBaoService>();
                //var fromDate = DateTime.MinValue;
                //var toDate = DateTime.MaxValue;
                //if (!string.IsNullOrWhiteSpace(request.Filterbctd.fromdate))
                //    fromDate = DateTime.ParseExact(request.Filterbctd.fromdate, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                //if (!string.IsNullOrWhiteSpace(request.Filterbctd.todate))
                //    toDate = DateTime.ParseExact(request.Filterbctd.todate, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var list = service.GetBaoCaotonghoptiendo(request.Filterbctd.maDViQly, request.Filterbctd.TenLoaiCanhBao, request.Filterbctd.fromdate, request.Filterbctd.todate);
   
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


        //Báo cáo chi tiết tiến độ (get)
       // [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaochitietgiamsattiendo")]
        public IHttpActionResult GetBaoCaoChiTietGiamSatTienDo(BaoCaoChiTietGiamSatTienDoReq request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;               
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();
                
                var list = canhBaoService.GetBaoCaoChiTietGiamSatTienDo(request.Filterbcgstd.maDViQly, request.Filterbcgstd.fromdate, request.Filterbcgstd.todate, request.Filterbcgstd.MaLoaiCanhBao);

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

        //Báo cáo chi tiết tiến độ (excel)
        //[JwtAuthentication]
        [HttpPost]
        [Route("exportbaocaochitietgiamsatiendo")]
        public IHttpActionResult exportbaocaochitietgiamsattiendo(BaoCaoChiTietGiamSatTienDoReq request)
        {
            try
            {
                DateTime synctime = DateTime.Today;
                ICanhBaoService canhBaoService = IoC.Resolve<ICanhBaoService>();

                var list = canhBaoService.GetBaoCaoChiTietGiamSatTienDo(request.Filterbcgstd.maDViQly, request.Filterbcgstd.fromdate, request.Filterbcgstd.todate, request.Filterbcgstd.MaLoaiCanhBao);

                string filetemplate = AppDomain.CurrentDomain.BaseDirectory + "templates/baocaochitietgiamsattiendo.xlsx";
                //  BÁO CÁO CHI TIẾT CÔNG TÁC GIÁM SÁT TIẾN ĐỘ GIẢI QUYẾT CẤP ĐIỆN TRUNG ÁP TỪ NGÀY … ĐẾN NGÀY  ... NĂM 202…
                string title = $"BÁO CÁO CHI TIẾT CÔNG TÁC GIÁM SÁT TIẾN ĐỘ GIẢI QUYẾT CẤP ĐIỆN TRUNG ÁP TỪ NGÀY {request.Filterbcgstd.fromdate} ĐẾN NGÀY {request.Filterbcgstd.todate}";

           
                FileInfo filetemp = new FileInfo(filetemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(filetemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 1].Value = title;
                    int row = 9;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.MaDViQuanLy;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.MaYeuCau;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TenKhachHang;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.DiaChi;
                        colval++;

                        ws.Cells[row, colval].Value = item.SDT;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongCongSuatDangKy;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NgayTiepNhan;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.HangMucCanhBao;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NguongCanhBao;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.TrangThai;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NgayGioGiamSat;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NguoiGiamSat;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NoiDungKhaoSat;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NoiDungXuLyYKienKH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.PhanHoi;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.XacMinhNguyenNhanChamGiaiQuyet;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.NDGhiNhanVaChuyenDonViXuLy;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.KetQua;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        colval++;

                        ws.Cells[row, colval].Value = item.GhiChu;
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


        //Báo cáo tổng hợp đánh giá mức độ (get)
        //[JwtAuthentication]
        [HttpPost]
        [Route("getbaocaothdanhgiamucdo")]
        public IHttpActionResult GetBaoCaoTongHopDanhGiaMucDo(BaoCaoTHDanhGiaMucDoHaiLong request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();


                //danh sách khảo sát có trạng thái chuyẻn khai thác 
                var list = service.GetBaoCaoTongHopDanhGiaMucDo(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách khảo sát có trạng thái trở ngại hoặc hết hạn TTDN
                var list1 = service.GetBaoCaoTongHopDanhGiaMucDo1(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách tổng số khảo sát có trạng thái = chuyển khai thác
                var chuyenKhaiThac = service.GetListChuyenKhaiThacTotal(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách tổng số khảo sát có trang thai = trở ngại hoặc hết hạn TTDN 
                var troNgai = service.GetListTroNgaiTotal(request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                var finalList = new { listSoLuongKhaoSatTrangThaiChuyenKhaiThac = list, listSoLuongKhaoSatTrangThaiTroNgai = list1 , tongSoKhaoSatTrangThaiChuyenKhaiThac = chuyenKhaiThac, tongSoKhaoSatTrangThaiTroNgai = troNgai };
                result.data = finalList;
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

        //Báo cáo tổng hợp đánh giá mức dộ (excel)
        //[JwtAuthentication]
        [HttpPost]
        [Route("exportbaocaothdanhgiamucdo")]
        public IHttpActionResult ExportBaoCaoTongHopDanhGiaMucDo(BaoCaoTHDanhGiaMucDoHaiLong request)
        {
            try
            {
                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();

                //danh sách khảo sát có trạng thái chuyẻn khai thác 
                var list = service.GetBaoCaoTongHopDanhGiaMucDo(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách khảo sát có trạng thái trở ngại hoặc hết hạn TTDN
                var list1 = service.GetBaoCaoTongHopDanhGiaMucDo1(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách tổng số khảo sát có trạng thái = chuyển khai thác
                var chuyenKhaiThac = service.GetListChuyenKhaiThacTotal(request.FilterDGiaDoHaiLong.madvi, request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);

                //danh sách tổng số khảo sát có trang thai = trở ngại hoặc hết hạn TTDN 
                var troNgai = service.GetListTroNgaiTotal(request.FilterDGiaDoHaiLong.fromdate, request.FilterDGiaDoHaiLong.todate, request.FilterDGiaDoHaiLong.HangMucKhaoSat);
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/BaoCaoTongHopDanhGiaMucDo.xlsx";

                string title = $"BÁO CÁO TỔNG HỢP ĐÁNH GIÁ MỨC ĐỘ HÀI LÒNG TRONG CÔNG TÁC CẤP ĐIỆN TRUNG ÁP TỪ NGÀY {request.FilterDGiaDoHaiLong.fromdate} ĐẾN NGÀY {request.FilterDGiaDoHaiLong.todate}";

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    ws.Cells[2, 1].Value = title;
                    int row = 8;
                    int row1 = 9;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.DonVi;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = "Chuyển khai thác";
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TongSoVuCoChenhLech;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienDeDangThuanTienCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienDeDangThuanTienKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienNhanhChongKipThoiCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienNhanhChongKipThoiKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienThaiDoChuyenNghiepCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.YCauCapDienThaiDoChuyenNghiepKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNTienDoKhaoSatCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNTienDoKhaoSatKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNMinhBachCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNMinhBachKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNChuDaoCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TTDNChuDaoKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuThuanTienCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuThuanTienKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuMinhBachCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuMinhBachKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuChuDaoCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NghiemThuChuDaoKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.ChiPhiCo;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;


                        ws.Cells[row, colval].Value = item.ChiPhiKhong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.RatKhongHaiLong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.KhongHaiLong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.BinhThuong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.HaiLong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.RatHaiLong;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;
                        row +=2;
                    }

                    foreach (var item in list1)
                    {
                        
                        int colval = 3;
                        

                        ws.Cells[row1, colval].Value = "Có trở ngại hoặc hết hạn TTĐN";
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TongSoVuCoChenhLech;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienDeDangThuanTienCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienDeDangThuanTienKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienNhanhChongKipThoiCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienNhanhChongKipThoiKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienThaiDoChuyenNghiepCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.YCauCapDienThaiDoChuyenNghiepKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNTienDoKhaoSatCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNTienDoKhaoSatKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNMinhBachCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNMinhBachKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNChuDaoCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.TTDNChuDaoKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuThuanTienCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuThuanTienKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuMinhBachCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuMinhBachKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuChuDaoCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.NghiemThuChuDaoKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.ChiPhiCo;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;


                        ws.Cells[row1, colval].Value = item.ChiPhiKhong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.RatKhongHaiLong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.KhongHaiLong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.BinhThuong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.HaiLong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row1, colval].Value = item.RatHaiLong;
                        ws.Cells[row1, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        row1 +=2;
                    }

                    row1 -= 1;
                    int colvalT = 2;
                    ws.Cells[row1, colvalT].Value = "Tổng Cộng";
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = "Chuyển khai thác";
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoVuCoChenhLech;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienDeDangThuanTienCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienDeDangThuanTienKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienNhanhChongKipThoiCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienNhanhChongKipThoiKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienThaiDoChuyenNghiepCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoYCauCapDienThaiDoChuyenNghiepKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNTienDoKhaoSatCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNTienDoKhaoSatKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNMinhBachCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNMinhBachKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNChuDaoCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoTTDNChuDaoKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuThuanTienCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuThuanTienKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuMinhBachCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuMinhBachKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuChuDaoCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoNghiemThuChuDaoKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoChiPhiCo;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoChiPhiKhong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoRatKhongHaiLong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoKhongHaiLong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoBinhThuong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoHaiLong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    ws.Cells[row1, colvalT].Value = chuyenKhaiThac.TongSoRatHaiLong;
                    ws.Cells[row1, colvalT].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT++;

                    row1++;
                    int colvalT1 = 3;

                    ws.Cells[row1, colvalT1].Value = "Có trở ngại hoặc hết hạn TTĐN";
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    
                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoVuCoChenhLech;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienDeDangThuanTienCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienDeDangThuanTienKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienNhanhChongKipThoiCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienNhanhChongKipThoiKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienThaiDoChuyenNghiepCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoYCauCapDienThaiDoChuyenNghiepKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNTienDoKhaoSatCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNTienDoKhaoSatKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNMinhBachCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNMinhBachKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNChuDaoCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoTTDNChuDaoKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuThuanTienCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuThuanTienKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuMinhBachCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuMinhBachKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuChuDaoCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoNghiemThuChuDaoKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoChiPhiCo;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoChiPhiKhong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoRatKhongHaiLong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoKhongHaiLong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoBinhThuong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoHaiLong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    ws.Cells[row1, colvalT1].Value = troNgai.TongSoRatHaiLong;
                    ws.Cells[row1, colvalT1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    colvalT1++;

                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("getbaocaothquahan")]
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

        //Báo cáo chi tiết đánh giá mức độ (excel)
        //[JwtAuthentication]
        [HttpPost]
        [Route("exportchitietmucdohailong")]
        public IHttpActionResult ExportBaoCaoChiTietMucDoHaiLong(BaoCaoChiTietMucDoHaiLongReq request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/BaoCaoChiTietMucDoHaiLong.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);

                var list = service.GetBaoCaoChiTietMucDoHaiLong(request.Filterbcmdhl.maDViQly, request.Filterbcmdhl.fromdate, request.Filterbcmdhl.todate, request.Filterbcmdhl.HangMucKhaoSat);
                string title = $"BÁO CÁO CHI TIẾT ĐÁNH GIÁ MỨC ĐỘ HÀI LÒNG TRONG CÔNG TÁC CẤP ĐIỆN TRUNG ÁP TỪ NGÀY {request.Filterbcmdhl.fromdate} ĐẾN NGÀY {request.Filterbcmdhl.todate}";
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 1].Value = title;

                    int row = 8;
                    int stt = 0;
                    foreach (var item in list)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_DVI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_YCAU;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_KH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.TEN_KH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DIA_CHI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DIEN_THOAI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.MUCDICH_SD_DIEN;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_TIEPNHAN.ToString();
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_HOANTHANH.ToString();
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.SO_NGAY_CT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.SO_NGAY_TH_ND;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        if (item.TRANGTHAI_GQ == 1) 
                        {
                            ws.Cells[row, colval].Value = "Kết thúc chuyển khai thác khách hàng";
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }
                        else
                        {
                            ws.Cells[row, colval].Value = "Kết thúc do có trở ngại";
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        ws.Cells[row, colval].Value = item.TONG_CONGSUAT_CD;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DGCD_TH_CHUONGTRINH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DGCD_TH_DANGKY;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DGCD_KH_PHANHOI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.DGCD_KH_PHANHOI - item.DGCD_TH_CHUONGTRINH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        if(item.DGYC_DK_DEDANG == 1)
                        {
                        ws.Cells[row, colval].Value = 1;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval+=2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if(item.DGYC_XACNHAN_NCHONG_KTHOI == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.DGYC_THAIDO_CNGHIEP == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval+=2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                         if(item.DGKS_TDO_KSAT == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.DGKS_MINH_BACH == 1) 
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.DGKS_CHU_DAO == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if(item.DGNT_THUAN_TIEN == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if(item.DGNT_MINH_BACH == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.DGNT_CHU_DAO == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.KSAT_CHI_PHI == 1)
                        {

                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval++;
                        }

                        if (item.DGHL_CAPDIEN == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 5;
                        }else if(item.DGHL_CAPDIEN == 2)
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 4;
                        }else if(item.DGHL_CAPDIEN == 3)
                        {
                            colval+=2;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 3;
                        }
                        else if (item.DGHL_CAPDIEN == 4)
                        {
                            colval += 3;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else if (item.DGHL_CAPDIEN == 5)
                        {
                            colval += 4;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval ++;
                        }
                        if(item.TRANGTHAI_GOI == 1)
                        {
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 3;
                        }else if(item.TRANGTHAI_GOI == 2)
                        {
                            colval++;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval += 2;
                        }
                        else if (item.TRANGTHAI_GOI == 3)
                        {
                            colval+=2;
                            ws.Cells[row, colval].Value = 1;
                            ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colval ++;
                        }
                        ws.Cells[row, colval].Value = item.NGAY.ToString();
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;                        

                        ws.Cells[row, colval].Value = item.NGUOI_KSAT;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.Y_KIEN_KH;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.NOIDUNG;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.PHAN_HOI;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        ws.Cells[row, colval].Value = item.GHI_CHU;
                        ws.Cells[row, colval].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        colval++;

                        row++;
                        
                    }
                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //Báo cáo chi tiết đánh giá mức độ (get)
        //[JwtAuthentication]
        [HttpPost]
        [Route("getchitietmucdohailong")]
        public IHttpActionResult GetBaoCaoChiTietMucDoHaiLong(BaoCaoChiTietMucDoHaiLongReq request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/BaoCaoChiTietMucDoHaiLong.xlsx";

                FileInfo fileTemp = new FileInfo(fileTemplate);

                var list = service.GetBaoCaoChiTietMucDoHaiLong(request.Filterbcmdhl.maDViQly, request.Filterbcmdhl.fromdate, request.Filterbcmdhl.todate, request.Filterbcmdhl.HangMucKhaoSat);

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
        [Route("exportbaocaothquahan")]
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
