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
                var list = service.GetSoLuongGui(model.Filterdashboardcanhbao.madvi);

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
                var list = service.GetSoLuongKhaoSat(Request.Filterdashboadkhaosat.madvi);

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
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var list = service.Getbieudo3(Request.Filtertgcd.donViQuanLy);
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

    }
}