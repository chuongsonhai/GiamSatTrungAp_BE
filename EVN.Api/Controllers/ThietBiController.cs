using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/thietbi")]
    public class ThietBiController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(ThietBiController));
        //khanh kiem thu GIT
        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(ThietBiRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
             
                IThietBiService service = IoC.Resolve<IThietBiService>();
                
                var list = service.GetByFilter(request.maDViQLy, request.MaYeuCau);
                var data = new List<ThietBiModel>();
                foreach (var item in list)
                    data.Add(new ThietBiModel(item));
                result.data = data;
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
        public IHttpActionResult Post([FromBody] DSachThietBiRequest request)
        {
            IThietBiService service = IoC.Resolve<IThietBiService>();
            ResponseResult result = new ResponseResult();
            try
            {
                var items = service.GetbyMaYCau(request.MaYeuCau);
                service.BeginTran();
                foreach (var item in items)
                {
                    service.Delete(item);
                }
                foreach (var item in request.Items)
                {
                    ThietBi tbi = new ThietBi();
                    tbi = item.ToEntity(tbi);
                    tbi.MaDViQLy = request.MaDViQLy;
                    tbi.MaYeuCau = request.MaYeuCau;
                    service.CreateNew(tbi);
                }
                service.CommitTran();
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                service.RolbackTran();
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }
    }
}