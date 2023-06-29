using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.IServices;
using FX.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/tientrinh")]
    public class TienTrinhController : ApiController
    {
        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(TienTrinhFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IDvTienTrinhService service = IoC.Resolve<IDvTienTrinhService>();
                request.Filter.maYCau = !string.IsNullOrWhiteSpace(request.Filter.maYCau) ? request.Filter.maYCau.Trim() : request.Filter.maYCau;
                var list = service.GetbyFilter(request.Filter.maYCau, request.Filter.keyword, pageindex, request.Paginator.pageSize, out total);
                IList<TienTrinhModel> data = new List<TienTrinhModel>();
                foreach (var item in list)
                {
                    data.Add(new TienTrinhModel(item));
                }

                result.total = total;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.data = new List<TienTrinhModel>();
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("export")]
        public IHttpActionResult Export(TienTrinhFilter request)
        {
            try
            {
                
                string fileTemplate = AppDomain.CurrentDomain.BaseDirectory + "Templates/ThongKe_TienDo.xlsx";
                IDvTienTrinhService service = IoC.Resolve<IDvTienTrinhService>();
                var list = service.GetForExport(request.maYCau, request.keyword);
                IList<TienTrinhModel> data = new List<TienTrinhModel>();
                foreach (var item in list)
                {
                    data.Add(new TienTrinhModel(item));
                }

                FileInfo fileTemp = new FileInfo(fileTemplate);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(fileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    int row = 4;
                    int stt = 0;
                    foreach (var item in data)
                    {
                        stt++;
                        int colval = 1;
                        ws.Cells[row, colval].Value = stt;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_YCAU_KNAI;
                        colval++;

                        ws.Cells[row, colval].Value = item.MA_DDO_DDIEN;

                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_BDAU;

                        colval++;

                        ws.Cells[row, colval].Value = item.NGAY_KTHUC;

                        colval++;

                        ws.Cells[row, colval].Value = item.NDUNG_XLY;

                        row++;
                    }

                    return Ok(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpGet]
        [Route("getlist/{maYeuCau}")]
        public IHttpActionResult GetList(string maYeuCau)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IDvTienTrinhService service = IoC.Resolve<IDvTienTrinhService>();
                var list = service.Query.Where(p => p.MA_YCAU_KNAI == maYeuCau).OrderBy(p => p.STT).ToList();
                IList<TienTrinhModel> data = new List<TienTrinhModel>();
                foreach (var item in list)
                {
                    data.Add(new TienTrinhModel(item));
                }

                result.total = list.Count();
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }
    }
}
