using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using EVN.Core.Models;
using EVN.Core.Repository;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DashboardController));
        //2.1.(GET) dashboard/canhbao
        //[JwtAuthentication]
        [HttpPost]
        [Route("dashboard/canhbao")]
        public IHttpActionResult GetCanhbao(CanhBaoFilterRequestdashboardcanhbao model)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();

                IList<CanhbaoModel> data = new List<CanhbaoModel>();
                var list = service.GetSoLuongGui(model.Filterdashboardcanhbao.fromdate, model.Filterdashboardcanhbao.todate);
                result.data = list;
                result.success = true;
                return Ok(result);
                //foreach (var item in list)
                //{
                //    data.Add(new CanhbaoModel(item));

                //}
                //result.total = list.Count();
                //result.data = data;
                //result.success = true;
                //return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                var mess = ex.Message;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        //2.2 (GET)/dashboard/khaosat
        //[JwtAuthentication]
        [HttpPost]
        [Route("dashboard/khaosat")]
        public IHttpActionResult GetKhaosat(XacNhanTroNgaiFilterkhaosatRequest Request)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                IXacNhanTroNgaiService service = IoC.Resolve<IXacNhanTroNgaiService>();
                var list = service.GetSoLuongKhaoSat(Request.Filterdashboadkhaosat.tuNgay, Request.Filterdashboadkhaosat.denNgay);

                //  result.total = list.Count();
                result.data = list;
                result.success = true;
                return Ok(result);

            }
            catch (Exception ex)
            {
                result.success = false;
                var mess = ex.Message;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }
       

        //2.3.(GET) dashboard/thoigiancapdien
        [HttpPost]
        [Route("dashboard/thoigiancapdien")]
        public IHttpActionResult GetThoigiancapdien(XacNhanTGCDReques Request)
        {
            ResponseResult result = new ResponseResult();
            try
            {

    
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
                if (!string.IsNullOrWhiteSpace(Request.Filtertgcd.tuNgay))
                    fromDate = DateTime.ParseExact(Request.Filtertgcd.tuNgay, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                if (!string.IsNullOrWhiteSpace(Request.Filtertgcd.denNgay))
                    toDate = DateTime.ParseExact(Request.Filtertgcd.denNgay, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);


                var list = service.GetThoigiancapdien(Request.Filtertgcd.donViQuanLy, fromDate, toDate);

                var listModel = new Thoigiancapdien(list);


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

    }
}