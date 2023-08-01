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
    [RoutePrefix("api/PhanhoiTraodoi")]
    public class PhanhoiTraodoiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(PhanhoiTraodoiController));

  

      

        //[JwtAuthentication]
        [HttpGet]
        [Route("delete")]
        public IHttpActionResult Delete([FromUri] int ID)
        {
            ResponseFileResult result = new ResponseFileResult();
            try
            {
                IPhanhoiTraodoiService service = IoC.Resolve<IPhanhoiTraodoiService>();
                var item = new PhanhoiTraodoi();
                item.ID = ID;
                //item.TenLoaiCanhBao = model.TenLoaiCanhBao;
                //item.ChuKyGui = model.ChuKyGui;
                //item.PhanLoai = model.PhanLoai;
                service.Delete(item);
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }



    }
}