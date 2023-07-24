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
    [RoutePrefix("api/dashboard")]
    public class CanhBaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CanhBaoController));

        //1.(GET) dashboard/canhbao
        //[JwtAuthentication]
        [HttpGet]
        [Route("dashboard/canhbao")]
        public IHttpActionResult GetCanhbao(string tungay, string denngay)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                ICanhBaoService service = IoC.Resolve<ICanhBaoService>();
                var list = service.GetbyCanhbao(tungay, denngay);
                IList<CanhbaoModel> data = new List<CanhbaoModel>();

                foreach (var item in list)
                {
                  data.Add(new CanhbaoModel(item));
              
                }
                result.total = list.Count();
                result.data = data;
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

        //2.(GET) dashboard/khaosat
        //[JwtAuthentication]
        [HttpGet]
        [Route("khaosat")]
        public IHttpActionResult GetKhaosat(string tungay, string denngay)
        {

            ResponseResult result = new ResponseResult();
            try
            {
                IKhaoSatService service = IoC.Resolve<IKhaoSatService>();
                var list = service.GetSoLuongKhaoSat(tungay, denngay);

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


        //3.(GET) dashboard/thoigiancapdien
        [HttpPost]
        [Route("GetThoigiancapdien")]
        public IHttpActionResult GetThoigiancapdien(string donViQuanLy, DateTime tungay, DateTime denngay)
        {
            ResponseResult result = new ResponseResult();
            try
            {

                DateTime synctime = DateTime.Today;
                IReportService service = IoC.Resolve<IReportService>();
                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MaxValue;
               

                var list = service.GetThoigiancapdien(donViQuanLy, tungay, denngay);
             
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
