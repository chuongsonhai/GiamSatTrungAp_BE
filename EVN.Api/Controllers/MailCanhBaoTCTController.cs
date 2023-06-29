using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/mailcanhbaotct")]
    public class MailCanhBaoTCTController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(MailCanhBaoTCTController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(MailCanhBaoTCTFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IMailCanhBaoTCTService service = IoC.Resolve<IMailCanhBaoTCTService>();

                var list = service.GetByFilter(request.Filter.tenNV,request.Filter.email, pageindex, request.Paginator.pageSize, out total);


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
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IMailCanhBaoTCTService service = IoC.Resolve<IMailCanhBaoTCTService>();
                var item = service.Getbykey(id);

                return Ok(item);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }

        [JwtAuthentication]
        [HttpPost]
        public IHttpActionResult Post([FromBody] MailCanhBaoTCT model)
        {
            try
            {
                IMailCanhBaoTCTService service = IoC.Resolve<IMailCanhBaoTCTService>();

                var entity = service.Getbykey(model.ID);
                if (entity == null)
                {
                    service.Save(model);
                    service.CommitChanges();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] MailCanhBaoTCT model)
        {
            try
            {
                IMailCanhBaoTCTService service = IoC.Resolve<IMailCanhBaoTCTService>();

                var entity = service.Getbykey(model.ID);
                if (entity != null)
                {
                    service.Save(model);
                    service.CommitChanges();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
