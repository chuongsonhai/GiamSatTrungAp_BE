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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(DashboardController));

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
    }
}